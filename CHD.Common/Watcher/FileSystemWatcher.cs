using System.IO;
using System.Threading;

namespace CHD.Common.Watcher
{
    public class FileSystemWatcher : IFileSystemWatcher
    {
        private readonly System.IO.FileSystemWatcher _watcher;

        private long _disposed = 0L;

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

        public event FileSystemEventHandler Changed
        {
            add
            {
                _watcher.Changed += value;
            }
            remove
            {
                _watcher.Changed -= value;
            }
        }

        public event FileSystemEventHandler Created
        {
            add
            {
                _watcher.Created += value;
            }
            remove
            {
                _watcher.Created -= value;
            }
        }

        public event FileSystemEventHandler Deleted
        {
            add
            {
                _watcher.Deleted += value;
            }
            remove
            {
                _watcher.Deleted -= value;
            }
        }

        public event RenamedEventHandler Renamed
        {
            add
            {
                _watcher.Renamed += value;
            }
            remove
            {
                _watcher.Renamed -= value;
            }
        }

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

        public FileSystemWatcher(
            string targetPath
            )
        {
            _watcher = new System.IO.FileSystemWatcher(targetPath);
        }

        public void Dispose()
        {
            if(Interlocked.Exchange(ref _disposed, 1L) == 0L)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
            }
        }
    }
}