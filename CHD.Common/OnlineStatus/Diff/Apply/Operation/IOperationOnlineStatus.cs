using CHD.Common.Operation;

namespace CHD.Common.OnlineStatus.Diff.Apply.Operation
{
    public interface IOperationOnlineStatus
    {
        void Start(
            IOperation operation
            );

        void EndWithSuccess(
            IOperation operation
            );

        void EndWithErrors(
            IOperation operation
            );
    }
}