using CHD.Common.FileSystem;
using CHD.Common.FileSystem.FFile;

namespace CHD.Common.Operation.FileOperation
{
    public interface IFileOperation : IOperation
    {
        IFile File
        {
            get;
        }
    }
}