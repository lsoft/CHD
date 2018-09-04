using CHD.Common.FileSystem;

namespace CHD.Common.Sync.Factory
{
    public interface ISynchronizerFactory
    {
        ISynchronizer CreateSynchronizer(
            IFileSystemConnector localConnector,
            IFileSystemConnector remoteConnector
            );
    }
}