using System;
using CHD.Common.Operation;
using CHD.Common.Serializer;

namespace CHD.Common.Sync.Report
{
    [Serializable]
    public sealed class SingleOperationReport
    {
        public SerializationVersionProvider<SingleOperationReport> SerializationVersion = new SerializationVersionProvider<SingleOperationReport>();

        public OperationTypeEnum Type
        {
            get;
            private set;
        }

        public string FullPath
        {
            get;
            private set;
        }

        public SingleOperationReport(
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
    }
}