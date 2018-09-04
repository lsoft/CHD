using CHD.Common.FileSystem;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.Operation.FileOperation;

namespace CHD.Common.Operation.FolderOperation
{
    public interface IFolderOperation : IOperation
    {
        IFolder Folder
        {
            get;
        }

        int FolderCount
        {
            get;
        }

        int FileCount
        {
            get;
        }

        bool IsByPathContains(
            IFileOperation operation
            );

        bool IsByPathContains(
            IFolderOperation operation
            );
    }
}