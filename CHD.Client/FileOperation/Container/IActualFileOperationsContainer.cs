using CHD.Client.FileOperation.Pusher;

namespace CHD.Client.FileOperation.Container
{
    internal interface IActualFileOperationsContainer
    {
        event FileOperationChangedDelegate FileOperationChangedEvent;

        void AddOperation(
            IFileOperation fileOperation,
            FileActionTypeEnum fileActionType
            );

        void StartOperation(
            IFileOperation fileOperation
            );

        void RemoveOperation(
            IFileOperation fileOperation
            );
    }
}