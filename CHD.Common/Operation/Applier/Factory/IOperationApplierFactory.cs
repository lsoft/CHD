using CHD.Common.FileSystem;

namespace CHD.Common.Operation.Applier.Factory
{
    public interface IOperationApplierFactory
    {
        IOperationApplier Create(
            IFileSystem fileSystem
            );
    }
}