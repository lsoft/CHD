using System;
using CHD.Common.Operation;

namespace CHD.Common.OnlineStatus.Diff.Apply.Report
{
    public sealed class OperationReport : IOperationReport
    {
        public IOperation Operation
        {
            get;
            private set;
        }

        public DateTime? StartTime
        {
            get;
            private set;
        }

        public DateTime? EndTime
        {
            get;
            private set;
        }

        public OperationReportStatusEnum Status
        {
            get;
            private set;
        }

        public event OnlineStatusChanged OnlineStatusEvent;

        public OperationReport(IOperation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            Operation = operation;
            Status = OperationReportStatusEnum.NotStarted;
        }

        public void Start()
        {
            StartTime = DateTime.Now;
            Status = OperationReportStatusEnum.InProgress;

            OnOnlineStatus();
        }

        public void EndWithSuccess()
        {
            EndTime = DateTime.Now;
            Status = OperationReportStatusEnum.SuccessfullyCompleted;

            OnOnlineStatus();
        }

        public void EndWithErrors()
        {
            EndTime = DateTime.Now;
            Status = OperationReportStatusEnum.CompletedWithErrors;

            OnOnlineStatus();
        }

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