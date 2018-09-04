using System;
using CHD.Common.FileSystem;
using CHD.Common.Sync.Factory;

namespace CHD.Common.Sync.Provider
{
    public sealed class SynchronizerProvider : ISynchronizerProvider
    {
        private readonly IFileSystemConnector _localConnector;
        private readonly IFileSystemConnector _remoteConnector;
        private readonly ISynchronizerFactory _synchronizerFactory;

        public SynchronizerProvider(
            IFileSystemConnector localConnector,
            IFileSystemConnector remoteConnector,
            ISynchronizerFactory synchronizerFactory
            )
        {
            if (localConnector == null)
            {
                throw new ArgumentNullException("localConnector");
            }
            if (remoteConnector == null)
            {
                throw new ArgumentNullException("remoteConnector");
            }
            if (synchronizerFactory == null)
            {
                throw new ArgumentNullException("synchronizerFactory");
            }

            _localConnector = localConnector;
            _remoteConnector = remoteConnector;
            _synchronizerFactory = synchronizerFactory;
        }

        public ISynchronizer CreateSynchronizer()
        {
            var sync = _synchronizerFactory.CreateSynchronizer(
                _localConnector,
                _remoteConnector
                );

            return
                sync;
        }
    }
}