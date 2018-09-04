using System;

namespace CHD.Common.Operation.Fixer
{
    public interface IFixer<T> : IFixer
    {
        T Result
        {
            get;
        }
    }

    public interface IFixer : IDisposable
    {
        void SafelyCommit(); //no exception should be raised at all

        void SafelyRevert(); //no exception should be raised at all
    }
}