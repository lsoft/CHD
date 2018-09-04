using System;
using CHD.Common.Sync.Report;

namespace CHD.Common.OnlineStatus.Sync
{
    public sealed class SyncOnline : ISyncOnlineStatus, ISyncOnlineReport
    {
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
            get
            {
                return
                    IsCompletedWithError || IsCompletedWithSuccess;
            }
        }

        public bool IsCompletedWithError
        {
            get;
            private set;
        }
        
        public bool IsCompletedWithSuccess
        {
            get;
            private set;
        }

        public DateTime? EndTime
        {
            get;
            private set;
        }

        public event OnlineStatusChanged OnlineStatusEvent;

        public void Start()
        {
            IsInProgress = true;
            IsCompletedWithError = false;
            IsCompletedWithSuccess = false;
            StartTime = DateTime.Now;
            EndTime = null;

            OnOnlineStatus();
        }

        public void EndWithSuccess(SyncReport report)
        {
            if (report == null)
            {
                throw new ArgumentNullException("report");
            }

            IsInProgress = false;
            IsCompletedWithSuccess = true;
            EndTime = DateTime.Now;

            OnOnlineStatus();
        }

        public void EndWithError()
        {
            IsInProgress = false;
            IsCompletedWithError = true;
            EndTime = DateTime.Now;

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