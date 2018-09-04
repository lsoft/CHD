using System;

namespace CHD.Common.OnlineStatus.Sync
{
    public interface ISyncOnlineReport
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

        bool IsCompletedWithError
        {
            get;
        }

        bool IsCompletedWithSuccess
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
