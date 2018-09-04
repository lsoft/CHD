using System.Collections.Generic;
using CHD.Common.Operation.FileOperation;
using CHD.Common.Operation.FolderOperation;
using CHD.Common.OperationLog;

namespace CHD.Common.Operation.Visitor
{
    public interface IOperationVisitor
    {
        void Visit(IFileOperation operation);

        void Visit(IFolderOperation operation);
    }
}