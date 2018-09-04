using System.Collections.Generic;
using CHD.Common.FileSystem.FFile;
using CHD.Common.Operation;
using CHD.Common.Operation.FileOperation;
using CHD.Common.Operation.FolderOperation;

namespace CHD.Common.OperationLog
{
    public interface IOperationLog
    {
        IReadOnlyList<IOperation> Operations
        {
            get;
        }

        IReadOnlyList<IFileOperation> FileOperations
        {
            get;
        }

        IReadOnlyList<IFolderOperation> FolderOperations
        {
            get;
        }

        int Count
        {
            get;
        }

        bool IsEmpty
        {
            get;
        }

        bool TryGetFileOperation(
            IFile file,
            out IFileOperation operation
            );

        void Dump(string header, IDisorderLogger logger);
    }
}