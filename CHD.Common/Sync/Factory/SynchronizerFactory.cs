using System;
using CHD.Common.Breaker;
using CHD.Common.Diff;
using CHD.Common.Diff.Applier;
using CHD.Common.Diff.Constructor;
using CHD.Common.FileSystem;
//using CHD.Common.KeyValueContainer.SyncedVersion;
using CHD.Common.Scanner;
using CHD.Common.Watcher;

namespace CHD.Common.Sync.Factory
{
    public sealed class SynchronizerFactory : ISynchronizerFactory
    {
        private readonly IScanner _scanner;
        private readonly IDiffConstructor _diffConstructor;
        private readonly IFileWatcherController _fileWatcherController;
        private readonly IBreaker _breaker;
        private readonly IDiffApplier _diffApplier;
        private readonly IDisorderLogger _logger;

        public SynchronizerFactory(
            IScanner scanner,
            IDiffConstructor diffConstructor,
            IFileWatcherController fileWatcherController,
            IBreaker breaker,
            IDiffApplier diffApplier,
            IDisorderLogger logger
            )
        {
            if (scanner == null)
            {
                throw new ArgumentNullException("scanner");
            }
            if (diffConstructor == null)
            {
                throw new ArgumentNullException("diffConstructor");
            }
            if (fileWatcherController == null)
            {
                throw new ArgumentNullException("fileWatcherController");
            }
            if (breaker == null)
            {
                throw new ArgumentNullException("breaker");
            }
            if (diffApplier == null)
            {
                throw new ArgumentNullException("diffApplier");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _scanner = scanner;
            _diffConstructor = diffConstructor;
            _fileWatcherController = fileWatcherController;
            _breaker = breaker;
            _diffApplier = diffApplier;
            _logger = logger;
        }

        public ISynchronizer CreateSynchronizer(
            IFileSystemConnector localConnector,
            IFileSystemConnector remoteConnector
            )
        {
            var result = new Synchronizer(
                _scanner,
                _diffConstructor,
                _fileWatcherController,
                _breaker,
                _diffApplier,
                localConnector,
                remoteConnector,
                _logger
                );

            return
                result;
        }
    }
}