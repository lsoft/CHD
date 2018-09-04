using System;
using System.Diagnostics;
using CHD.Common.FileSystem;
using CHD.Common.FileSystem.FFile;
using CHD.Common.Operation.Applier;
using CHD.Common.Operation.Fixer;
using CHD.Common.Operation.Visitor;

namespace CHD.Common.Operation.FileOperation
{
    [DebuggerDisplay("{Type}: {FullPath}")]
    public sealed class FileOperation : IFileOperation
    {
        public string HumanReadableDescription
        {
            get
            {
                return
                    string.Format(
                        "{0}: {1}",
                        Type,
                        File.FullPath
                        );
            }
        }

        public string FullPath
        {
            get
            {
                return
                    File.FullPath;
            }
        }

        public bool IsFileOperation
        {
            get
            {
                return true;
            }
        }

        public bool IsFolderOperation
        {
            get
            {
                return false;
            }
        }

        public OperationTypeEnum Type
        {
            get;
            private set;
        }

        public IFile File
        {
            get;
            private set;
        }

        public FileOperation(
            OperationTypeEnum type,
            IFile file
            )
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            Type = type;
            File = file;
        }

        public bool WithSameTarget(string fullPath)
        {
            if (fullPath == null)
            {
                throw new ArgumentNullException("fullPath");
            }

            return
                this.File.IsSame(fullPath);
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

            dumper.LogOperation(
                Type,
                File.FullPath
                );
        }
    }
}