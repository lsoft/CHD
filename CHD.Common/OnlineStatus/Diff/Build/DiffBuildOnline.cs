using System;
using System.Collections.Generic;

namespace CHD.Common.OnlineStatus.Diff.Build
{
    public sealed class DiffBuildOnline : IDiffBuildOnlineReport, IDiffBuildOnlineStatus
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