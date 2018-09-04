using CHD.Common.FileSystem;
using CHD.Common.Operation.Applier;
using CHD.Common.Operation.Fixer;
using CHD.Common.Operation.Visitor;

namespace CHD.Common.Operation
{
    public interface IOperation
    {
        OperationTypeEnum Type
        {
            get;
        }

        string HumanReadableDescription
        {
            get;
        }

        string FullPath
        {
            get;
        }

        bool IsFileOperation
        {
            get;
        }

        bool IsFolderOperation
        {
            get;
        }

        bool WithSameTarget(string fullPath);

        IOperationFixer Apply(
            IFileSystemCopier sourceFileSystemCopier,
            IOperationApplier applier
            );

        void Accept(
            IOperationVisitor visitor
            );

        void Dump(IOperationDumper dumper);
    }
}