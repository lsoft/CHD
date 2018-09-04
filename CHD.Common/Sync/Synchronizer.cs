using System;
using System.Threading;
using CHD.Common.Breaker;
using CHD.Common.Diff;
using CHD.Common.Diff.Applier;
using CHD.Common.Diff.Conflict;
using CHD.Common.Diff.Constructor;
using CHD.Common.FileSystem;
using CHD.Common.Scanner;
using CHD.Common.Sync.Report;
using CHD.Common.Watcher;

namespace CHD.Common.Sync
{
    public sealed class Synchronizer : ISynchronizer
    {
        private readonly IScanner _scanner;
        private readonly IDiffConstructor _diffConstructor;
        private readonly IFileWatcherController _fileWatcherController;
        private readonly IBreaker _breaker;
        private readonly IDiffApplier _diffApplier;
        private readonly IFileSystemConnector _localConnector;
        private readonly IFileSystemConnector _remoteConnector;
        private readonly IDisorderLogger _logger;

        private readonly object _locker = new object();

        public Synchronizer(
            IScanner scanner,
            IDiffConstructor diffConstructor,
            IFileWatcherController fileWatcherController,
            IBreaker breaker,
            IDiffApplier diffApplier,
            IFileSystemConnector localConnector,
            IFileSystemConnector remoteConnector,
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
            if (localConnector == null)
            {
                throw new ArgumentNullException("localConnector");
            }
            if (remoteConnector == null)
            {
                throw new ArgumentNullException("remoteConnector");
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
            _localConnector = localConnector;
            _remoteConnector = remoteConnector;
            _logger = logger;
        }

        public SyncReport Sync()
        {
            if (!Monitor.TryEnter(_locker))
            {
                return
                    SyncReport.AlreadyPerforming;
            }
            
            try
            {
                var result = DoSync();

                return
                    result;
            }
            finally
            {
                Monitor.Exit(_locker);
            }
        }

        private SyncReport DoSync()
        {
            SyncReport result = null;

            _logger.LogMessage("------------------  Syncing started  -----------------");

            try
            {
                //reset break status before sync procedure begins
                _breaker.ResetBreak();

                _fileWatcherController.StartWatching();

                #region check for change identifiers

                var localActual = _scanner.Scan();
                var localActualChangeIdentifier = localActual.CalculateChangeIdentifier();

                var remoteStructure = _remoteConnector.ReadStructureContainer();
                var remoteChangeIdentifier = remoteStructure.Last.GetChangeIdentifier();

                if (localActualChangeIdentifier == remoteChangeIdentifier)
                {
                    //различий нет

                    _logger.LogMessage("--  Nothing to sync because of change identifiers  --");

                    return
                        SyncReport.Empty;
                }

                #endregion

                _logger.LogFormattedMessage(
                    "Open remote file system: {0}",
                    _remoteConnector.Name
                    );

                using (var remote = _remoteConnector.Open(remoteStructure))
                {
                    _logger.LogFormattedMessage(
                        "Open local file system: {0}",
                        _localConnector.Name
                        );

                    using (var local = _localConnector.Open())
                    {
                        //do diff
                        IConflictDescription conflictDescription;
                        IDiff diff = _diffConstructor.BuildDiff(
                            localActual,
                            local.LastStructure.RootFolder,
                            remote.LastStructure.RootFolder,
                            out conflictDescription
                            );

                        if (diff.IsEmpty)
                        {
                            _logger.LogMessage("-----------------  Nothing to sync  -----------------");

                            return
                                SyncReport.Empty;
                        }

                        //check for conficts
                        conflictDescription.RaiseExceptionIfConfictExists();

                        //no conficts exists

                        //update local structure with user actions
                        local.SaveActualStructure(
                            localActual
                            );

                        //do sync
                        _diffApplier.Apply(
                            local,
                            remote,
                            diff
                            );

                        //stop watching the local file system before commit phase
                        _fileWatcherController.StopWatching();

                        //fix the changes
                        result = _diffApplier.Commit();
                        remote.SafelyCommit();
                        local.SafelyCommit();

                        //everything is ok
                    } //close local file system
                } //close remote file system

                result.Dump(_logger);
            }
            finally
            {
                //stop watching the local file system because of syncing is finished
                _fileWatcherController.StopWatching();

                //reverting
                _diffApplier.Revert();

                _logger.LogMessage("------------------  Sync completed  -----------------");
            }

            return
                result;
        }
    }
}