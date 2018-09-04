using System;
using System.Runtime.Serialization;
using CHD.Common.Operation;

namespace CHD.WcfChannel.Journal
{
    [DataContract]
    public sealed class WcfSingleOperationReport
    {
        [DataMember]
        public OperationTypeEnum Type
        {
            get;
            private set;
        }

        [DataMember]
        public string FullPath
        {
            get;
            private set;
        }

        public WcfSingleOperationReport(
            OperationTypeEnum type,
            string fullPath
            )
        {
            if (fullPath == null)
            {
                throw new ArgumentNullException("fullPath");
            }
            Type = type;
            FullPath = fullPath;
        }

        public bool ChangesExists(WcfSingleOperationReport report)
        {
            if (report == null)
            {
                throw new ArgumentNullException("report");
            }

            if (this.Type != report.Type)
            {
                return true;
            }
            if (this.FullPath != report.FullPath)
            {
                return true;
            }

            return
                false;
        }
    }
}