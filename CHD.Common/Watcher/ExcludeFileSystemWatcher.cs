using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CHD.FileSystem.Algebra;

namespace CHD.FileSystem.Watcher
{
    public class ExcludeFileSystemWatcher : IFileSystemWatcher, IExcluder
    {
        private readonly string _targetPath;
        private readonly IFileSystemWatcher _watcher;

        private int _disposed = 0;

        private readonly object _suffixLock = new object();
        private readonly HashSet<Suffix> _excludedSuffixes = new HashSet<Suffix>();

        public bool EnableRaisingEvents
        {
            get
            {
                return
                    _watcher.EnableRaisingEvents;
            }
            set
            {
                _watcher.EnableRaisingEvents = value;
            }
        }

        public NotifyFilters NotifyFilter
        {
            get
            {
                return
                    _watcher.NotifyFilter;
            }
            set
            {
                _watcher.NotifyFilter = value;
            }
        }

        public bool IncludeSubdirectories
        {
            get
            {
                return
                    _watcher.IncludeSubdirectories;
            }
            set
            {
                _watcher.IncludeSubdirectories = value;
            }
        }

        public string Path
        {
            get
            {
                return
                    _watcher.Path;
            }
            set
            {
                _watcher.Path = value;
            }
        }

        public event FileSystemEventHandler Changed;

        public event FileSystemEventHandler Created;

        public event FileSystemEventHandler Deleted;

        public event RenamedEventHandler Renamed;

        public event ErrorEventHandler Error
        {
            add
            {
                _watcher.Error += value;
            }
            remove
            {
                _watcher.Error -= value;
            }
        }

        public ExcludeFileSystemWatcher(
            string targetPath,
            IFileSystemWatcher fileSystemWatcher
            )
        {
            if (targetPath == null)
            {
                throw new ArgumentNullException("targetPath");
            }
            if (fileSystemWatcher == null)
            {
                throw new ArgumentNullException("fileSystemWatcher");
            }

            _targetPath = targetPath;
            _watcher = fileSystemWatcher;

            Subscribe();
        }

        public void AddToExcluded(
            string filepath
            )
        {
            var suffix = PathAlgebra.GetSuffix(
                filepath,
                _targetPath
                );

            lock (_suffixLock)
            {
                _excludedSuffixes.Add(suffix);
            }
        }

        public void RemoveFromExcluded(
            string filepath
            )
        {
            var suffix = PathAlgebra.GetSuffix(
                filepath,
                _targetPath
                );

            lock (_suffixLock)
            {
                _excludedSuffixes.Remove(suffix);
            }
        }

        public void Dispose()
        {
            var disposed = Interlocked.Exchange(ref _disposed, 1);
            if (disposed == 0)
            {
                Unsubscribe();
            }
        }

        private void Subscribe()
        {
            _watcher.Changed += WatcherOnChanged;
            _watcher.Created += WatcherOnCreated;
            _watcher.Deleted += WatcherOnDeleted;
            _watcher.Renamed += WatcherOnRenamed;
        }

        private void Unsubscribe()
        {
            _watcher.Changed -= WatcherOnChanged;
            _watcher.Created -= WatcherOnCreated;
            _watcher.Deleted -= WatcherOnDeleted;
            _watcher.Renamed -= WatcherOnRenamed;
        }

        private void WatcherOnChanged(
            object sender,
            FileSystemEventArgs fileSystemEventArgs
            )
        {
            if (!IsExcluded(fileSystemEventArgs.FullPath))
            {
                Changed(
                    sender,
                    fileSystemEventArgs
                    );
            }
        }

        private void WatcherOnRenamed(
            object sender,
            RenamedEventArgs renamedEventArgs
            )
        {
            if (!IsExcluded(renamedEventArgs.FullPath))
            {
                Renamed(
                    sender,
                    renamedEventArgs
                    );
            }
        }

        private void WatcherOnDeleted(
            object sender, 
            FileSystemEventArgs fileSystemEventArgs
            )
        {
            if (!IsExcluded(fileSystemEventArgs.FullPath))
            {
                Deleted(
                    sender,
                    fileSystemEventArgs
                    );
            }
        }

        private void WatcherOnCreated(
            object sender, 
            FileSystemEventArgs fileSystemEventArgs
            )
        {
            if (!IsExcluded(fileSystemEventArgs.FullPath))
            {
                Created(
                    sender,
                    fileSystemEventArgs
                    );
            }
        }

        private bool IsExcluded(
            string filepath
            )
        {
            var suffix = PathAlgebra.GetSuffix(
                filepath,
                _targetPath
                );

            lock (_suffixLock)
            {
                var result = _excludedSuffixes.Contains(suffix);

                return
                    result;
            }
        }

    }
}