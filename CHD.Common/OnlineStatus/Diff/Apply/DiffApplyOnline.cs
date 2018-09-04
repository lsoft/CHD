using System;
using System.Collections.Generic;
using System.Linq;
using CHD.Common.Diff;
using CHD.Common.OnlineStatus.Diff.Apply.Operation;
using CHD.Common.OnlineStatus.Diff.Apply.Report;
using CHD.Common.Operation;

namespace CHD.Common.OnlineStatus.Diff.Apply
{
    public sealed class DiffApplyOnline : IDiffApplyOnlineReport, IDiffApplyOnlineStatus, IOperationOnlineStatus
    {
        private readonly List<OperationReport> _operationReports = new List<OperationReport>();

        public bool IsInProgress
        {
            get;
            private set;
        }

        public DateTime? StartTime
        {
            get;
            private set;
        }

        public bool IsCompleted
        {
            get;
            private set;
        }

        public DateTime? EndTime
        {
            get;
            private set;
        }

        public IReadOnlyList<IOperationReport> OperationReports
        {
            get
            {
                return
                    _operationReports;
            }
        }

        public event OnlineStatusChanged OnlineStatusEvent;


        #region IDiffApplyOnlineStatus

        public void Start(IDiff diff)
        {
            if (diff == null)
            {
                throw new ArgumentNullException("diff");
            }

            _operationReports.Clear();

            _operationReports.AddRange(
                diff.LocalLog.Operations.Select(j => new OperationReport(j))
                );
            _operationReports.AddRange(
                diff.RemoteLog.Operations.Select(j => new OperationReport(j))
                );

            IsInProgress = true;
            IsCompleted = false;
            StartTime = DateTime.Now;
            EndTime = null;

            OnOnlineStatus();
        }

        public void End()
        {
            IsInProgress = false;
            IsCompleted = true;
            EndTime = DateTime.Now;

            OnOnlineStatus();
        }

        #endregion

        #region IOperationOnlineStatus

        public void Start(IOperation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            var r = _operationReports.FirstOrDefault(j => ReferenceEquals(j.Operation, operation));
            if (r != null)
            {
                r.Start();
            }
        }

        public void EndWithSuccess(IOperation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            var r = _operationReports.FirstOrDefault(j => ReferenceEquals(j.Operation, operation));
            if (r != null)
            {
                r.EndWithSuccess();
            }
        }

        public void EndWithErrors(IOperation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            var r = _operationReports.FirstOrDefault(j => ReferenceEquals(j.Operation, operation));
            if (r != null)
            {
                r.EndWithErrors();
            }
        }

        #endregion

        private void OnOnlineStatus()
        {
            OnlineStatusChanged handler = OnlineStatusEvent;
            if (handler != null)
            {
                handler();
            }
        }

    }
}