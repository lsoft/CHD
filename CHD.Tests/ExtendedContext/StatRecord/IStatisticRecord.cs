using System;

namespace CHD.Tests.ExtendedContext.StatRecord
{
    public interface IStatisticRecord : IDisposable
    {
        void Log();
    }
}