using System;
using CHD.Common.FileSystem;
using CHD.Common.Sync.Factory;
using CHD.Common.Sync.Report.Journal;

namespace CHD.Common.Sync.Storeable
{
    public sealed class StoreableSynchronizerFactory : ISynchronizerFactory
    {
        private readonly ISynchronizerFactory _synchronizerFactory;
        private readonly ISyncJournal _container;

        public StoreableSynchronizerFactory(
            ISynchronizerFactory synchronizerFactory,
            ISyncJournal container
            )
        {
            if (synchronizerFactory == null)
            {
                throw new ArgumentNullException("synchronizerFactory");
            }
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            _synchronizerFactory = synchronizerFactory;
            _container = container;
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

            var result = new StoreableSynchronizer(
                synchronizer,
                _container
                );

            return
                result;
        }
    }
}