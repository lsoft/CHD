using System;
using System.Collections.Generic;
using CHD.Common.FileSystem.FFile;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.Operation.FileOperation;
using CHD.Common.Operation.FolderOperation;

namespace CHD.Common.PathComparer
{
    public interface IPathComparerProvider
    {
        StringComparison Comparison
        {
            get;
        }

        StringComparer Comparer
        {
            get;
        }

        IEqualityComparer<IFolderOperation> GetFolderOperationComparer();

        IEqualityComparer<IFileOperation> GetFileOperationComparer();

        IEqualityComparer<IFolder> GetFolderComparer();

        IEqualityComparer<IFile> GetFileComparer();
    }
}