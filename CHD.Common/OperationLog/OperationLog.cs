using System;
using System.Collections.Generic;
using CHD.Common.FileSystem.FFile;
using CHD.Common.Operation;
using CHD.Common.Operation.FileOperation;
using CHD.Common.Operation.FolderOperation;

namespace CHD.Common.OperationLog
{
    public sealed class OperationLog : IOperationLog
    {
        private readonly List<IOperation> _operations;
        private readonly List<IFileOperation> _fileOperations;
        private readonly List<IFolderOperation> _folderOperations;

        public IReadOnlyList<IOperation> Operations
        {
            get
            {
                return
                    _operations;
            }
        }

        public IReadOnlyList<IFileOperation> FileOperations
        {
            get
            {
                return
                    _fileOperations;
            }
        }

        public IReadOnlyList<IFolderOperation> FolderOperations
        {
            get
            {
                return
                    _folderOperations;
            }
        }

        public int Count
        {
            get
            {
                return
                    _operations.Count;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return
                    _operations.Count == 0;
            }
        }



        public bool TryGetFileOperation(
            IFile file,
            out IFileOperation operation
            )
        {
            foreach (var fo in _fileOperations)
            {
                if (fo.WithSameTarget(file.FullPath))
                {
                    operation = fo;
                    return true;
                }
            }

            operation = null;
            return false;
        }

        public void Dump(string header, IDisorderLogger logger)
        {
            if (header == null)
            {
                throw new ArgumentNullException("header");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            var dumper = new LoggerOperationDumper(logger);

            logger.LogFormattedMessage(
                "{0} operation count: {1}{2}",
                header,
                this.Count,
                this.Count > 0 ? ":" : string.Empty
                );

            foreach (var op in _operations)
            {
                op.Dump(dumper);
            }
        }

        public OperationLog(
            )
        {
            _operations = new List<IOperation>();
            _fileOperations = new List<IFileOperation>();
            _folderOperations = new List<IFolderOperation>();
        }

        public void AddOperation(IFileOperation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            _operations.Add(operation);
            _fileOperations.Add(operation);
        }

        public void AddOperation(IFolderOperation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            _operations.Add(operation);
            _folderOperations.Add(operation);
        }






        public static readonly OperationLog Empty;

        static OperationLog()
        {
            Empty = new OperationLog();
        }

    }
}