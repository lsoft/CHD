namespace CHD.Push.FileChangeWatcher
{
    public interface IFileChangeWatcher
    {
        void AsyncStart();
        
        void Stop();
    }
}