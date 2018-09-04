namespace CHD.Common.Operation.Fixer
{
    public interface IOperationFixer
    {
        /// <summary>
        /// �������� ��� ��������� �������� �������
        /// </summary>
        IOperation Operation
        {
            get;
        }

        void SafelyCommit(); //no exception should be raised at all

        void SafelyRevert(); //no exception should be raised at all
    }
}