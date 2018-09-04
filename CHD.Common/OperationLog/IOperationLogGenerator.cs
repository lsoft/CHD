using CHD.Common.FileSystem.FFolder;

namespace CHD.Common.OperationLog
{
    public interface IOperationLogGenerator
    {
        IOperationLog Generate(
            IFolder previous,
            IFolder next
            );
    }
}