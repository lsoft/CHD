using CHD.Common.Others;

namespace CHD.Common.Breaker
{
    public abstract class BaseBreaker : IReadBreaker
    {
        public abstract bool ShouldBreak
        {
            get;
        }

        public void RaiseExceptionIfBreak(
            string message = ""
            )
        {
            if (!ShouldBreak)
            {
                return;
            }

            throw new CHDException(
                message,
                CHDExceptionTypeEnum.BreakSignal
                );
        }
    }
}