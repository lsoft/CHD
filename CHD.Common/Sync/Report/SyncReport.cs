using System;
using CHD.Common.Others;
using CHD.Common.Serializer;

namespace CHD.Common.Sync.Report
{
    [Serializable]
    public sealed class SyncReport //: ISyncReport
    {
        public SerializationVersionProvider<SyncReport> SerializationVersion = new SerializationVersionProvider<SyncReport>();

        public DateTime SyncDate
        {
            get;
            private set;
        }

        public SyncResultEnum SyncResult
        {
            get;
            private set;
        }

        public IFileSystemSyncReport Local
        {
            get;
            private set;
        }

        public IFileSystemSyncReport Remote
        {
            get;
            private set;
        }

        public void Dump(IDisorderLogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            logger.LogDateDetailed(
                SyncDate,
                "Sync time: {0}"
                );

            logger.LogFormattedMessage(
                "Sync result: {0}",
                SyncResult
                );

            if (SyncResult.In(SyncResultEnum.AlreadyPerforming, SyncResultEnum.NoChangesDetected))
            {
                return;
            }

            Local.Dump(logger);
            Remote.Dump(logger);
        }

        public static SyncReport Empty
        {
            get
            {
                return 
                    new SyncReport(
                        SyncResultEnum.NoChangesDetected,
                        FileSystemSyncReport.Empty,
                        FileSystemSyncReport.Empty
                        );
            }
        }

        public static SyncReport AlreadyPerforming
        {
            get
            {
                return
                    new SyncReport(
                        SyncResultEnum.AlreadyPerforming,
                        FileSystemSyncReport.Empty,
                        FileSystemSyncReport.Empty
                        );
            }
        }

        public SyncReport(
            SyncResultEnum syncResult,
            IFileSystemSyncReport local, 
            IFileSystemSyncReport remote
            )
        {
            if (local == null)
            {
                throw new ArgumentNullException("local");
            }
            if (remote == null)
            {
                throw new ArgumentNullException("remote");
            }

            SyncDate = DateTime.Now;

            SyncResult = syncResult;
            Local = local;
            Remote = remote;
        }


    }
}