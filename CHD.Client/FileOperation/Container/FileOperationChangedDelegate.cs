namespace CHD.Client.FileOperation.Container
{
    internal delegate void FileOperationChangedDelegate(
        bool created,
        ActualFileOperationWrapper operationWrapper
        );
}