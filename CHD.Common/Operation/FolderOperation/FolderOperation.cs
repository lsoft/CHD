using System;
using System.Diagnostics;
using CHD.Common.FileSystem;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.Operation.Applier;
using CHD.Common.Operation.FileOperation;
using CHD.Common.Operation.Fixer;
using CHD.Common.Operation.Visitor;

namespace CHD.Common.Operation.FolderOperation
{
    [DebuggerDisplay("{Type}: {FullPath}")]
    public sealed class FolderOperation : IFolderOperation
    {
        public string HumanReadableDescription
        {
            get
            {
                return
                    string.Format(
                        "{0}: {1}",
                        Type,
                        Folder.FullPath
                        );
            }
        }

        public string FullPath
        {
            get
            {
                return
                    Folder.FullPath;
            }
        }

        public bool IsFileOperation
        {
            get
            {
                return false;
            }
        }

        public bool IsFolderOperation
        {
            get
            {
                return true;
            }
        }


        public OperationTypeEnum Type
        {
            get;
            private set;
        }

        public IFolder Folder
        {
            get;
            private set;
        }

        public int FolderCount
        {
            get
            {
                return
                    Folder.RecursivelyFolderCount;
            }
        }

        public int FileCount
        {
            get
            {
                return
                    Folder.RecursivelyFileCount;
            }
        }


        public FolderOperation(
            OperationTypeEnum type,
            IFolder folder
            )
        {
            if (folder == null)
            {
                throw new ArgumentNullException("folder");
            }

            Type = type;
            Folder = folder;
        }

        public bool WithSameTarget(string fullPath)
        {
            if (fullPath == null)
            {
                throw new ArgumentNullException("fullPath");
            }

            return
                Folder.IsSame(fullPath);
        }

        public bool IsByPathContains(IFileOperation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }
            var result = this.Folder.IsByPathContains(operation.File);

            return
                result;
        }

        public bool IsByPathContains(IFolderOperation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }
            var result = this.Folder.IsByPathContains(operation.Folder);

            return
                result;
        }

        public IOperationFixer Apply(
            IFileSystemCopier sourceFileSystemCopier,
            IOperationApplier applier
            )
        {
            if (sourceFileSystemCopier == null)
            {
                throw new ArgumentNullException("sourceFileSystemCopier");
            }
            if (applier == null)
            {
                throw new ArgumentNullException("applier");
            }

            var result = applier.ApplyOperation(
                sourceFileSystemCopier,
                this
                );

            return
                result;
        }

        public void Accept(IOperationVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException("visitor");
            }

            visitor.Visit(this);
        }

        public void Dump(IOperationDumper dumper)
        {
            if (dumper == null)
            {
                throw new ArgumentNullException("dumper");
            }

            DumpFolder(
                Type,
                Folder,
                dumper
                );
        }

        private static void DumpFolder(
            OperationTypeEnum type,
            IFolder folder,
            IOperationDumper dumper
            )
        {
            dumper.LogOperation(
                type,
                folder.FullPath
                );

            foreach (var f in folder.Files)
            {
                dumper.LogOperation(
                    type,
                    f.FullPath
                    );
            }

            foreach (var f in folder.Folders)
            {
                DumpFolder(
                    type,
                    f,
                    dumper
                    );
            }
        }
    }
}