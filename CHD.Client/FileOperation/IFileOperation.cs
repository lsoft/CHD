using CHD.Client.FileOperation.Pusher;
using CHD.FileSystem.Algebra;

namespace CHD.Client.FileOperation
{
    public interface IFileOperation
    {
        Suffix FilePathSuffix
        {
            get;
        }

        float Progress
        {
            get;
        }

        event ProgressChangedDelegate ProgressChangedEvent;
    }
}