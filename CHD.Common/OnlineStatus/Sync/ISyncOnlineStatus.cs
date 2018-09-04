using CHD.Common.Sync.Report;

namespace CHD.Common.OnlineStatus.Sync
{
    public interface ISyncOnlineStatus
    {
        void Start(
            );

        void EndWithSuccess(
            SyncReport report
            );

        void EndWithError(
            );

    }
}