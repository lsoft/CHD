using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using CHD.Common.OnlineStatus.Diff.Apply.Report;

namespace CHD.WcfChannel.Sync
{
    [DataContract]
    public sealed class WcfDiffApplyReport
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
        public List<WcfOperationReport> Operations
        {
            get;
            private set;
        }

        public WcfDiffApplyReport()
        {
            IsInProgress = false;
            IsCompleted = false;
            Operations = new List<WcfOperationReport>();
        }

        public WcfDiffApplyReport(bool isInProgress, bool isCompleted, IReadOnlyList<IOperationReport> operations)
        {
            IsInProgress = isInProgress;
            IsCompleted = isCompleted;

            Operations = new List<WcfOperationReport>();
            Operations.AddRange(operations.Select(j => new WcfOperationReport(
                j.Operation.Type,
                new List<string> { j.Operation.FullPath },
                j.Status
                )));
            //Operations.AddRange(
            //    new []
            //    {
            //        new WcfOperationReport(
            //            OperationTypeEnum.Recreate,
            //            new List<string>  { "target0" },
            //            OperationReportStatusEnum.SuccessfullyCompleted
            //            ),
            //        new WcfOperationReport(
            //            OperationTypeEnum.Recreate,
            //            new List<string>  { "target1" },
            //            OperationReportStatusEnum.InProgress
            //            ),
            //        new WcfOperationReport(
            //            OperationTypeEnum.Recreate,
            //            new List<string>  { "target2" },
            //            OperationReportStatusEnum.NotStarted
            //            ),
            //    }
            //    );
        }

        public bool ChangesExists(WcfDiffApplyReport report)
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

            if (this.Operations.Count != report.Operations.Count)
            {
                return true;
            }

            var zip = this.Operations.Zip(report.Operations, (a, b) => new Tuple<WcfOperationReport,WcfOperationReport>(a, b));

            if (zip.Any(j => j.Item1.ChangesExists(j.Item2)))
            {
                return true;
            }

            return
                false;
        }
    }
}