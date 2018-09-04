using System;

namespace CHD.Common.OnlineStatus.Diff.Build
{
    public interface IDiffBuildOnlineReport
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

        event OnlineStatusChanged OnlineStatusEvent;
    }
}
