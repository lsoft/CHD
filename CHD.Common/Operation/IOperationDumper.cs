namespace CHD.Common.Operation
{
    public interface IOperationDumper
    {
        void LogOperation(
            OperationTypeEnum type,
            string fullPath
            );
    }
}