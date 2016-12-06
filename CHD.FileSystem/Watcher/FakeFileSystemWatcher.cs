using System.IO;

namespace CHD.FileSystem.Watcher
{
    public class FakeFileSystemWatcher : IFileSystemWatcher
    {
        public bool EnableRaisingEvents
        {
            get;
            set;
        }

        public NotifyFilters NotifyFilter
        {
            get;
            set;
        }

        public bool IncludeSubdirectories
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }

        public event FileSystemEventHandler Changed;
        public event FileSystemEventHandler Created;
        public event FileSystemEventHandler Deleted;
        public event ErrorEventHandler Error;
        public event RenamedEventHandler Renamed;

        public void Dispose()
        {
            //nohting to do
        }
    }
}