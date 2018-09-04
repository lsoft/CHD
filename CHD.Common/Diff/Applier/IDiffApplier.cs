//using CHD.Common.KeyValueContainer.SyncedVersion;
using CHD.Common.FileSystem;
using CHD.Common.Sync.Report;

namespace CHD.Common.Diff.Applier
{
    public interface IDiffApplier
    {
        void Apply(
            IFileSystem local,
            IFileSystem remote,
            IDiff diff
            );

        SyncReport Commit();

        void Revert();
    }
}