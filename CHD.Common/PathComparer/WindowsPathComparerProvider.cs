using System;
using System.Collections.Generic;
using CHD.Common.FileSystem.FFile;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.Operation;
using CHD.Common.Operation.FileOperation;
using CHD.Common.Operation.FolderOperation;

namespace CHD.Common.PathComparer
{
    /// <summary>
    /// TODO: для всяких линуксов по другому сравнивать - создать типа LinuxPathComparerProvider
    /// </summary>
    [Serializable]
    public sealed class WindowsPathComparerProvider : IPathComparerProvider
    {
        public StringComparison Comparison
        {
            get
            {
                return
                    StringComparison.OrdinalIgnoreCase;
            }
        }

        public StringComparer Comparer
        {
            get
            {
                return
                    StringComparer.OrdinalIgnoreCase;
            }
        }

        public WindowsPathComparerProvider(
            )
        {
        }


        public IEqualityComparer<IFolderOperation> GetFolderOperationComparer()
        {
            return
                new FolderOperationComparer(this.Comparer);
        }

        public IEqualityComparer<IFileOperation> GetFileOperationComparer()
        {
            return
                new FileOperationComparer(this.Comparer);
        }

        public IEqualityComparer<IFolder> GetFolderComparer()
        {
            return
                new FolderComparer(this.Comparer);
        }

        public IEqualityComparer<IFile> GetFileComparer()
        {
            return
                new FileComparer(this.Comparer);
        }

    }
}
