using System;
using System.Collections.Generic;
using CHD.Common.OnlineStatus.Diff.Apply.Report;

namespace CHD.Common.OnlineStatus.Diff.Apply
{
    public interface IDiffApplyOnlineReport
    {
        bool IsInProgress
        {
            get;
        }

        DateTime? StartTime
        {
            get;
        }

        bool IsCompleted
        {
            get;
        }

        DateTime? EndTime
        {
            get;
        }

        IReadOnlyList<IOperationReport> OperationReports
        {
            get;
        }

        event OnlineStatusChanged OnlineStatusEvent;
    }
}