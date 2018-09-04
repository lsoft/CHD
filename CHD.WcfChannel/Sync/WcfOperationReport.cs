using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using CHD.Common.OnlineStatus.Diff.Apply.Report;
using CHD.Common.Operation;

namespace CHD.WcfChannel.Sync
{
    [DataContract]
    public sealed class WcfOperationReport
    {
        [DataMember]
        public OperationTypeEnum Type
        {
            get;
            private set;
        }

        public string Target
        {
            get
            {
                return
                    Targets.FirstOrDefault();
            }
        }

        [DataMember]
        public List<string> Targets
        {
            get;
            private set;
        }

        [DataMember]
        public OperationReportStatusEnum Status
        {
            get;
            private set;
        }

        public WcfOperationReport(OperationTypeEnum type, List<string> targets, OperationReportStatusEnum status)
        {
            Type = type;
            Targets = targets;
            Status = status;
        }

        public bool ChangesExists(WcfOperationReport report)
        {
            if (this.Type != report.Type)
            {
                return true;
            }

            if (this.Status != report.Status)
            {
                return true;
            }

            if (this.Targets.Count != report.Targets.Count)
            {
                return true;
            }

            var zip = this.Targets.Zip(report.Targets, (a, b) => new Tuple<string, string>(a, b));

            if (zip.Any(j => string.Compare(j.Item1, j.Item2) != 0))
            {
                return true;
            }

            return
                false;
        }
    }
}