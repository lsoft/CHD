using CHD.Common.FileSystem;
using CHD.Common.Operation.FileOperation;
using CHD.Common.Operation.Fixer;
using CHD.Common.Operation.FolderOperation;

namespace CHD.Common.Operation.Applier
{
    public interface IOperationApplier
    {
        IOperationFixer ApplyOperation(
            IFileSystemCopier sourceFileSystemCopier,
            IFileOperation operation
            );

        IOperationFixer ApplyOperation(
            IFileSystemCopier sourceFileSystemCopier,
            IFolderOperation operation
            );
    }
}
