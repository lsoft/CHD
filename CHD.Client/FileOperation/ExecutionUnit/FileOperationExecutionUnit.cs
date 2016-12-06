using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHD.Client.FileOperation.Container;
using CHD.Client.FileOperation.Pusher;
using CHD.Common.Logger;
using CHD.FileSystem.Algebra;
using CHD.Pull.Components;

namespace CHD.Client.FileOperation.ExecutionUnit
{
    internal class FileOperationExecutionUnit : IExecutionUnit, IFileOperation
    {
        private readonly IActualFileOperationsContainer _actualFileOperationsContainer;
        private readonly IExecutionUnit _executionUnit;
        private readonly IDisorderLogger _logger;

        public long Order
        {
            get
            {
                return
                    _executionUnit.Order;
            }
        }

        public Suffix FilePathSuffix
        {
            get
            {
                return
                    _executionUnit.FilePathSuffix;
            }
        }

        public float Progress
        {
            get;
            private set;
        }

        public event ProgressChangedDelegate ProgressChangedEvent;

        public FileOperationExecutionUnit(
            IActualFileOperationsContainer actualFileOperationsContainer,
            IExecutionUnit executionUnit,
            IDisorderLogger logger
            )
        {
            if (actualFileOperationsContainer == null)
            {
                throw new ArgumentNullException("actualFileOperationsContainer");
            }
            if (executionUnit == null)
            {
                throw new ArgumentNullException("executionUnit");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _actualFileOperationsContainer = actualFileOperationsContainer;
            _executionUnit = executionUnit;
            _logger = logger;

            _actualFileOperationsContainer.AddOperation(
                this,
                FileActionTypeEnum.Download
                );

        }

        public void PerformOperation(
            Action<int, int> progressChangeFunc = null
            )
        {
            Action<int, int> pcf;

            if (progressChangeFunc != null)
            {
                pcf = (cnt, total) =>
                {
                    progressChangeFunc(cnt, total);

                    ProgressChange(cnt, total);
                };
            }
            else
            {
                pcf = ProgressChange;
            }


            _actualFileOperationsContainer.StartOperation(
                this
                );

            try
            {
                _executionUnit.PerformOperation(pcf);
            }
            finally
            {
                _actualFileOperationsContainer.RemoveOperation(
                    this
                    );
            }
        }

        public void Dispose()
        {
            _executionUnit.Dispose();
        }

        private void ProgressChange(
            int cnt,
            int total
            )
        {
            Progress = cnt/(float) total;

            OnProgressChanged(Progress);
        }

        protected virtual void OnProgressChanged(float progress)
        {
            ProgressChangedDelegate handler = ProgressChangedEvent;
            if (handler != null)
            {
                try
                {
                    handler(progress);
                }
                catch (Exception excp)
                {
                    _logger.LogException(excp);
                }
            }
        }

    }
}
