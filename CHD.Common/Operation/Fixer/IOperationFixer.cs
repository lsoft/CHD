namespace CHD.Common.Operation.Fixer
{
    public interface IOperationFixer
    {
        /// <summary>
        /// операция для локальной файловой системы
        /// </summary>
        IOperation Operation
        {
            get;
        }

        void SafelyCommit(); //no exception should be raised at all

        void SafelyRevert(); //no exception should be raised at all
    }
}