using CHD.Common.Sync.Report;

namespace CHD.Common.Sync
{
    public interface ISynchronizer
    {
        SyncReport Sync();
    }
}