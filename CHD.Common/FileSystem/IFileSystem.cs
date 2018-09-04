using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using CHD.Common.FileSystem.FFolder;
using CHD.Common.Operation;
using CHD.Common.Operation.FileOperation;
using CHD.Common.PathComparer;
using CHD.Common.Structure;
using CHD.Common.Structure.Container;

namespace CHD.Common.FileSystem
{
    public interface IFileSystem : IDisposable
    {
        IPathComparerProvider PathComparerProvider
        {
            get;
        }

        IStructureContainer StructureContainer
        {
            get;
        }

        IStoredStructure LastStructure
        {
            get;
        }


        IFolder RootFolder
        {
            get;
        }

        IFileSystemCopier Copier
        {
            get;
        }

        IFileSystemExecutor Executor
        {
            get;
        }

        void SaveActualStructure(
            IFolder actualRoot
            );

        void SafelyCommit();
    }
}