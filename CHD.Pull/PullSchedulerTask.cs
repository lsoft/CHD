using System;
using System.Collections.Generic;
using System.Threading;
using CHD.Common.KeyValueContainer.Order;
using CHD.Common.Logger;
using CHD.Dynamic.Scheduler.Task;
using CHD.Graveyard.ExclusiveAccess;
using CHD.Graveyard.Graveyard;
using CHD.Graveyard.RemoteFileState;
using CHD.Pull.Components;
using CHD.Pull.Components.Factory;

namespace CHD.Pull
{
    public class PullSchedulerTask : ITask
    {
        private readonly IGraveyard _graveyard;
        private readonly IOrderContainer _orderContainer;
        private readonly IExecutionUnitFactory _executionUnitFactory;
        private readonly int _pullTimeoutAfterSuccessMsec;
        private readonly int _pullTimeoutAfterFailureMsec;
        private readonly IDisorderLogger _logger;

        private bool _previousSuccess;

        public Guid TaskGuid
        {
            get;
            private set;
        }

        public long MicrosecondsBetweenAwakes
        {
            get
            {
                return
                    _previousSuccess ? _pullTimeoutAfterSuccessMsec * 1000 : _pullTimeoutAfterFailureMsec * 1000;
            }
        }

        public PullSchedulerTask(
            IGraveyard graveyard,
            IOrderContainer orderContainer,
            IExecutionUnitFactory executionUnitFactory,
            int pullTimeoutAfterSuccessMsec,
            int pullTimeoutAfterFailureMsec,
            IDisorderLogger logger
            )
        {
            if (graveyard == null)
            {
                throw new ArgumentNullException("graveyard");
            }
            if (orderContainer == null)
            {
                throw new ArgumentNullException("orderContainer");
            }
            if (executionUnitFactory == null)
            {
                throw new ArgumentNullException("executionUnitFactory");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _graveyard = graveyard;
            _orderContainer = orderContainer;
            _executionUnitFactory = executionUnitFactory;
            _pullTimeoutAfterSuccessMsec = pullTimeoutAfterSuccessMsec;
            _pullTimeoutAfterFailureMsec = pullTimeoutAfterFailureMsec;
            _logger = logger;

            TaskGuid = Guid.NewGuid();
        }

        public void Execute(
            Func<bool> unexpectedBreakFunc,
            out bool needToRepeat
            )
        {
            if (unexpectedBreakFunc == null)
            {
                throw new ArgumentNullException("unexpectedBreakFunc");
            }

            var success = CheckForUpdates(
                unexpectedBreakFunc
                );

            _previousSuccess = success;

            needToRepeat = true;
        }

        #region private code

        private bool CheckForUpdates(
            Func<bool> unexpectedBreakFunc
            )
        {
            var result = false;

            IExclusiveAccess exclusiveAccess;
            if (_graveyard.TryGetExclusiveAccess(out exclusiveAccess))
            {
                try
                {
                    var currentLastOrder = _orderContainer.Order;

                    var fileStates = exclusiveAccess.GetSnapshotSince(
                        currentLastOrder
                        );

                    if (fileStates.Count > 0)
                    {
                        //for additional safety do sorting in order of Order property :)
                        fileStates.Sort(new ByOrderPropertyComparer());

                        var units = new List<IExecutionUnit>();
                        try
                        {
                            //1st phase: Locking and Exclusion
                            units = CreateUnits(
                                fileStates
                                );

                            //2nd phase: Safely Performing File Actions
                            PerformOperations(
                                unexpectedBreakFunc,
                                units
                                );

                            //PerformOperations may stopped early due to application shutdown
                            //so we need to check a stop singal
                            if (unexpectedBreakFunc())
                            {
                                //should break the procedure

                                return
                                    false;
                            }

                        }
                        finally
                        {
                            //safely close units
                            CloseUnitsAndCleanup(units);
                        }
                    }

                    result = true;
                }
                finally
                {
                    exclusiveAccess.Close();
                }
            }

            return
                result;
        }

        private void PerformOperations(
            Func<bool> unexpectedBreakFunc,
            List<IExecutionUnit> units
            )
        {
            if (units == null)
            {
                throw new ArgumentNullException("units");
            }

            //2nd phase: Safely Performing File Actions
            foreach (var unit in units)
            {
                //perform file operation: delete file or replace with new body
                unit.PerformOperation();

                //store operation order into local container
                _orderContainer.Order = unit.Order;

                //performing is a time-consuming operation
                //so we need to check a stop singal
                if (unexpectedBreakFunc())
                {
                    //should break the procedure

                    return;
                }
            }
        }

        private List<IExecutionUnit> CreateUnits(
            List<IRemoteFileState> fileStates
            )
        {
            if (fileStates == null)
            {
                throw new ArgumentNullException("fileStates");
            }

            var units = new List<IExecutionUnit>();

            //1st phase: Locking and Exclusion
            try
            {
                foreach (var fileState in fileStates)
                {
                    var unit = _executionUnitFactory.Create(
                        fileState
                        );

                    units.Add(unit);
                }
            }
            catch
            {
                //safely close units
                CloseUnitsAndCleanup(units);

                throw;
            }

            foreach (var fs in fileStates)
            {
                _logger.LogFormattedMessage(
                    "File {0} prepared for local processing initiated by remote changes",
                    fs.FilePathSuffix
                    );
            }

            return
                units;
        }

        private static void CloseUnitsAndCleanup(
            List<IExecutionUnit> units
            )
        {
            if (units == null)
            {
                throw new ArgumentNullException("units");
            }

            foreach (var unit in units)
            {
                unit.Dispose();
            }

            units.Clear();
        }

        #endregion
    }
}
