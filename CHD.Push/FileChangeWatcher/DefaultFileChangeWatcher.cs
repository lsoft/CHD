using System;
using System.IO;
using System.Threading;
using CHD.Common.Logger;
using CHD.FileSystem.FileWrapper;
using CHD.FileSystem.Watcher;
using CHD.Push.ActivityPool;

namespace CHD.Push.FileChangeWatcher
{
    public class DefaultFileChangeWatcher : IFileChangeWatcher, IDisposable
    {
        private readonly string _watchFolderPath;
        private readonly IFileWrapperFactory _fileWrapperFactory;
        private readonly IActivityPool _activityPool;
        private readonly IDisorderLogger _logger;

        private IFileSystemWatcher _watcher;

        private volatile bool _started = false;
        private volatile bool _stopped = false;

        public DefaultFileChangeWatcher(
            string watchFolderPath,
            IFileSystemWatcher watcher,
            IFileWrapperFactory fileWrapperFactory,
            IActivityPool activityPool,
            IDisorderLogger logger
            )
        {
            if (watchFolderPath == null)
            {
                throw new ArgumentNullException("watchFolderPath");
            }
            if (watcher == null)
            {
                throw new ArgumentNullException("watcher");
            }
            if (fileWrapperFactory == null)
            {
                throw new ArgumentNullException("fileWrapperFactory");
            }
            if (activityPool == null)
            {
                throw new ArgumentNullException("activityPool");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _watchFolderPath = watchFolderPath;
            _logger = logger;

            _watcher = watcher;
            _fileWrapperFactory = fileWrapperFactory;
            _activityPool = activityPool;
        }

        public void AsyncStart()
        {
            if (_started)
            {
                return;
            }
            if (_stopped)
            {
                return;
            }

            _watcher.IncludeSubdirectories = true;
            _watcher.NotifyFilter =
                NotifyFilters.FileName
                //| NotifyFilters.DirectoryName
                //| NotifyFilters.LastAccess
                | NotifyFilters.LastWrite
                //| NotifyFilters.CreationTime
                ;

            // Add event handlers.
            _watcher.Created += OnCreated;
            _watcher.Changed += OnChanged;
            _watcher.Renamed += OnRenamed;
            _watcher.Deleted += OnDeleted;
            _watcher.Error += OnError;

            _watcher.EnableRaisingEvents = true;

            _started = true;
        }

        public void Stop()
        {
            SafelyStop();
        }

        public void Dispose()
        {
            SafelyStop();
        }

        private void SafelyStop()
        {
            if (!_stopped)
            {
                _stopped = true;

                if (_started)
                {
                    var watcher = Interlocked.Exchange(ref _watcher, null);
                    if (watcher != null)
                    {
                        try
                        {
                            watcher.EnableRaisingEvents = false;
                            watcher.Dispose();
                        }
                        catch (Exception excp)
                        {
                            _logger.LogException(excp, "Exception has been suppressed");
                        }
                    }
                }
            }
        }


        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            _logger.LogFormattedMessage(
                "{0}: {1}",
                e.ChangeType,
                e.FullPath
                );

            var fileWrapper = _fileWrapperFactory.CreateFile(
                e.FullPath,
                _watchFolderPath
                );

            _activityPool.ApplyActivity(
                ActivityTypeEnum.CreateOrChange,
                fileWrapper
                );
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            _logger.LogFormattedMessage(
                "{0}: {1}",
                e.ChangeType,
                e.FullPath
                );

            var fileWrapper = _fileWrapperFactory.CreateFile(
                e.FullPath,
                _watchFolderPath
                );

            _activityPool.ApplyActivity(
                ActivityTypeEnum.CreateOrChange,
                fileWrapper
                );
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            _logger.LogFormattedMessage(
                "{0}: {1} -> {2}",
                e.ChangeType,
                e.OldFullPath,
                e.FullPath
                );


            var fileWrapper = _fileWrapperFactory.CreateFile(
                e.OldFullPath,
                _watchFolderPath
                );

            _activityPool.ApplyActivity(
                ActivityTypeEnum.Delete,
                fileWrapper
                );

            var file2Wrapper = _fileWrapperFactory.CreateFile(
                e.FullPath,
                _watchFolderPath
                );

            _activityPool.ApplyActivity(
                ActivityTypeEnum.CreateOrChange,
                file2Wrapper
                );
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            _logger.LogFormattedMessage(
                "{0}: {1}",
                e.ChangeType,
                e.FullPath
                );

            var fileWrapper = _fileWrapperFactory.CreateFile(
                e.FullPath,
                _watchFolderPath
                );

            _activityPool.ApplyActivity(
                ActivityTypeEnum.Delete,
                fileWrapper
                );
        }


        private void OnError(object sender, ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

    }
}
