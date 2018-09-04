using CHD.Common.FileSystem;

namespace CHD.Common.Operation.Applier.Factory
{
    public sealed class OperationApplierFactory : IOperationApplierFactory
    {
        public IOperationApplier Create(IFileSystem fileSystem)
        {
            var result = new OperationApplier(
                fileSystem
                );

            return result;
        }
    }
}