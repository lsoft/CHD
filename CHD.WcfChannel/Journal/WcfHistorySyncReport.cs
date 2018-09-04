using System;
using System.Runtime.Serialization;
using CHD.Common.Sync.Report;

namespace CHD.WcfChannel.Journal
{
    [DataContract]
    public sealed class WcfHistorySyncReport
    {
        [DataMember]
        public DateTime SyncDate
        {
            get;
            private set;
        }

        [DataMember]
        public SyncResultEnum SyncResult
        {
            get;
            private set;
        }

        [DataMember]
        public WcfFileSystemSyncReport Local
        {
            get;
            private set;
        }

        [DataMember]
        public WcfFileSystemSyncReport Remote
        {
            get;
            private set;
        }

        public WcfHistorySyncReport(
            DateTime syncDate,
            SyncResultEnum syncResult,
            WcfFileSystemSyncReport local,
            WcfFileSystemSyncReport remote
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

            SyncDate = syncDate;
            SyncResult = syncResult;
            Local = local;
            Remote = remote;
        }


        public bool ChangesExists(WcfHistorySyncReport report)
        {
            if (this.SyncDate != report.SyncDate)
            {
                return true;
            }

            if (this.SyncResult != report.SyncResult)
            {
                return true;
            }

            if (this.Local.ChangesExists(report.Local))
            {
                return true;
            }

            if (this.Remote.ChangesExists(report.Remote))
            {
                return true;
            }

            return false;
        }
    }
}