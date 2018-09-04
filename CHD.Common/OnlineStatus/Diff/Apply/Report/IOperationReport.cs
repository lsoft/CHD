using System;
using CHD.Common.Operation;

namespace CHD.Common.OnlineStatus.Diff.Apply.Report
{
    public interface IOperationReport
    {
        IOperation Operation
        {
            get;
        }

        DateTime? StartTime
        {
            get;
        }

        DateTime? EndTime
        {
            get;
        }

        OperationReportStatusEnum Status
        {
            get;
        }

        event OnlineStatusChanged OnlineStatusEvent;
    }
}