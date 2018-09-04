using System;
using System.Runtime.Serialization;

namespace CHD.WcfChannel.Sync
{
    [DataContract]
    public sealed class WcfSyncReport
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

        [DataMember]
        public WcfDiffBuildReport DiffBuildReport
        {
            get;
            private set;
        }

        [DataMember]
        public WcfDiffApplyReport DiffApplyReport
        {
            get;
            private set;
        }

        public WcfSyncReport()
        {
            IsInProgress = false;
            IsCompleted = false;
            DiffBuildReport = new WcfDiffBuildReport();
            DiffApplyReport = new WcfDiffApplyReport();
        }

        public WcfSyncReport(
            bool isInProgress, 
            bool isCompleted, 
            WcfDiffBuildReport diffBuildReport, 
            WcfDiffApplyReport diffApplyReport
            )
        {
            IsInProgress = isInProgress;
            IsCompleted = isCompleted;
            DiffBuildReport = diffBuildReport;
            DiffApplyReport = diffApplyReport;
        }

        public bool ChangesExists(WcfSyncReport report)
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

            if (this.DiffBuildReport.ChangesExists(report.DiffBuildReport))
            {
                return true;
            }

            if (this.DiffApplyReport.ChangesExists(report.DiffApplyReport))
            {
                return true;
            }


            return
                false;
        }
    }
}