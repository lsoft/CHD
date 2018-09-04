using System;
using System.Runtime.Serialization;

namespace CHD.WcfChannel.Sync
{
    [DataContract]
    public sealed class WcfDiffBuildReport
    {
        [DataMember]
        public bool IsInProgress
        {
            get;
            private set;
        }

        [DataMember]
        public bool IsCompleted
        {
            get;
            private set;
        }

        public WcfDiffBuildReport()
        {
            IsInProgress = false;
            IsCompleted = false;
        }

        public WcfDiffBuildReport(bool isInProgress, bool isCompleted)
        {
            IsInProgress = isInProgress;
            IsCompleted = isCompleted;
        }

        public bool ChangesExists(WcfDiffBuildReport report)
        {
            if (report == null)
            {
                throw new ArgumentNullException("report");
            }

            if (this.IsInProgress != report.IsInProgress)
            {
                return true;
            }

            if (this.IsCompleted != report.IsCompleted)
            {
                return true;
            }

            return
                false;
        }
    }
}