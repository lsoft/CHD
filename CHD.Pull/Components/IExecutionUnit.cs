using System;
using CHD.FileSystem.Algebra;

namespace CHD.Pull.Components
{
    public interface IExecutionUnit : IDisposable
    {
        long Order
        {
            get;
        }

        Suffix FilePathSuffix
        {
            get;
        }

        void PerformOperation(
            Action<int, int> progressChangeFunc = null
            );
    }
}