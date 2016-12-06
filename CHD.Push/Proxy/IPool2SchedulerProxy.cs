using CHD.FileSystem.FileWrapper;
using CHD.Push.ActivityPool;

namespace CHD.Push.Proxy
{
    public interface IPool2SchedulerProxy
    {
        void AddTask(
            ActivityTypeEnum activity, 
            IFileWrapper fileWrapper
            );

        void CancelTask(
            IFileWrapper fileWrapper
            );
    }
}