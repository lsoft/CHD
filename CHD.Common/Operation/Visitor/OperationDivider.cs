using System;
using System.Collections.Generic;
using CHD.Common.Operation.FileOperation;
using CHD.Common.Operation.FolderOperation;

namespace CHD.Common.Operation.Visitor
{
    public sealed class OperationDivider : IOperationVisitor
    {
        private readonly List<DividerPair<IFileOperation>> _fileOperations;
        private readonly List<DividerPair<IFolderOperation>> _folderOperations;

        private int _index;

        public IReadOnlyList<DividerPair<IFileOperation>> FileOperations
        {
            get
            {
                return
                    _fileOperations;
            }
        }

        public IReadOnlyList<DividerPair<IFolderOperation>> FolderOperations
        {
            get
            {
                return
                    _folderOperations;
            }
        }

        public OperationDivider()
        {
            _fileOperations = new List<DividerPair<IFileOperation>>();
            _folderOperations = new List<DividerPair<IFolderOperation>>();
            _index = 0;
        }

        public void Accept(
            IReadOnlyCollection<IOperation> operations
            )
        {
            if (operations == null)
            {
                throw new ArgumentNullException("operations");
            }

            foreach (var o in operations)
            {
                o.Accept(this);
            }
        }


        public void Visit(IFileOperation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }
            _fileOperations.Add(
                new DividerPair<IFileOperation>(
                    _index++,
                    operation
                    )
                );
        }

        public void Visit(IFolderOperation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            _folderOperations.Add(
                new DividerPair<IFolderOperation>(
                    _index++,
                    operation
                    )
                );
        }

        public sealed class DividerPair<TOperation>
            where TOperation : IOperation
        {
            public int Index
            {
                get;
                private set;
            }

            public TOperation Operation
            {
                get;
                private set;
            }

            public DividerPair(int index, TOperation operation)
            {
                Index = index;
                Operation = operation;
            }
        }
    }
}