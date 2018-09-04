using System;
using CHD.Common.FileSystem;
using CHD.Common.OnlineStatus.Sync;
using CHD.Common.Sync.Factory;

namespace CHD.Common.Sync.Online
{
    public sealed class OnlineSynchronizerFactory : ISynchronizerFactory
    {
        private readonly ISynchronizerFactory _synchronizerFactory;
        private readonly ISyncOnlineStatus _onlineStatus;

        public OnlineSynchronizerFactory(
            ISynchronizerFactory synchronizerFactory,
            ISyncOnlineStatus onlineStatus
            )
        {
            if (synchronizerFactory == null)
            {
                throw new ArgumentNullException("synchronizerFactory");
            }
            if (onlineStatus == null)
            {
                throw new ArgumentNullException("onlineStatus");
            }

            _synchronizerFactory = synchronizerFactory;
            _onlineStatus = onlineStatus;
        }

        public ISynchronizer CreateSynchronizer(
            IFileSystemConnector localConnector,
            IFileSystemConnector remoteConnector
            )
        {
            var synchronizer = _synchronizerFactory.CreateSynchronizer(
                localConnector,
                remoteConnector
                );

            var result = new OnlineSynchronizer(
                synchronizer,
                _onlineStatus
                );

            return
                result;
        }
    }
}