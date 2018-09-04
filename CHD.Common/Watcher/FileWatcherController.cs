using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CHD.Common.Breaker;
using CHD.Common.PathComparer;

namespace CHD.Common.Watcher
{
    public interface IFileWatcherController
    {
        void StartWatching();

        void StopWatching();
    }

    public sealed class FileWatcherController : IFileWatcherController
    {
        private const long NotWorking = 0L;
        private const long Working = 1L;

        private readonly IDisorderLogger _logger;

        private readonly IFileSystemWatcher _watcher;
        private readonly IBreaker _breaker;

        private long _status = NotWorking;

        public FileWatcherController(
            IFileSystemWatcher watcher,
            IBreaker breaker,
            IPathComparerProvider pathComparerProvider,
            IDisorderLogger logger
            )
        {
            if (watcher == null)
            {
                throw new ArgumentNullException("watcher");
            }
            if (breaker == null)
            {
                throw new ArgumentNullException("breaker");
            }
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _watcher = watcher;
            _breaker = breaker;
            _logger = logger;
        }


        public void StartWatching()
        {
            if (Interlocked.CompareExchange(ref _status, Working, NotWorking) == NotWorking)
            {
                _watcher.IncludeSubdirectories = true;
                _watcher.NotifyFilter =
                    NotifyFilters.FileName
                    | NotifyFilters.DirectoryName
                    //| NotifyFilters.LastAccess
                    | NotifyFilters.LastWrite
                    //| NotifyFilters.CreationTime
                    ;

                // Add event handlers.
                _watcher.Created += OnCreated;
                _watcher.Changed += OnChanged;
                _watcher.Renamed += OnRenamed;
                _watcher.Deleted += OnDeleted;

                _watcher.EnableRaisingEvents = true;
            }
        }

        public void StopWatching()
        {
            if (Interlocked.CompareExchange(ref _status, NotWorking, Working) == Working)
            {
                DoStop();
            }
        }

        private void DoStop()
        {
            _watcher.Created -= OnCreated;
            _watcher.Changed -= OnChanged;
            _watcher.Renamed -= OnRenamed;
            _watcher.Deleted -= OnDeleted;

            _watcher.EnableRaisingEvents = false;
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            var message = string.Format(
                "OnCreated -> {0}: {1}",
                e.ChangeType,
                e.FullPath
                );

            _logger.LogFormattedMessage(
                message
                );

            FilterAndProcess(
                message
                );
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            var message = string.Format(
                "OnChanged -> {0}: {1}",
                e.ChangeType,
                e.FullPath
                );

            _logger.LogFormattedMessage(
                message
                );

            FilterAndProcess(
                message
                );
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            var message = string.Format(
                "OnRenamed -> {0}: {1} -> {2}",
                e.ChangeType,
                e.OldFullPath,
                e.FullPath
                );

            _logger.LogFormattedMessage(
                message
                );

            FilterAndProcess(
                message
                );
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            var message = string.Format(
                "OnDeleted -> {0}: {1}",
                e.ChangeType,
                e.FullPath
                );

            _logger.LogFormattedMessage(
                message
                );

            FilterAndProcess(
                message
                );
        }

        private void FilterAndProcess(
            string message
            )
        {
            _breaker.FireBreak(string.Format("[{0}]", message));
        }
    }
}
