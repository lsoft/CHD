using CHD.FileSystem.FileWrapper;

namespace CHD.Push.ActivityPool
{
    public interface IActivityPool
    {
        void ApplyActivity(
            ActivityTypeEnum activity,
            IFileWrapper fileWrapper
            );
    }
}