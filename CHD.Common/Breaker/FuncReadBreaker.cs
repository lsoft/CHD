using System;

namespace CHD.Common.Breaker
{
    public sealed class FuncReadBreaker<TException> : IReadBreaker
        where TException: Exception
    {
        private readonly Func<bool> _shouldBreakFunc;

        public bool ShouldBreak
        {
            get
            {
                return
                    _shouldBreakFunc();
            }
        }

        public FuncReadBreaker(
            Func<bool> shouldBreakFunc 
            )
        {
            if (shouldBreakFunc == null)
            {
                throw new ArgumentNullException("shouldBreakFunc");
            }
            _shouldBreakFunc = shouldBreakFunc;
        }

        public void RaiseExceptionIfBreak(string message = "")
        {
            if (!ShouldBreak)
            {
                return;
            }

            throw (TException)Activator.CreateInstance(
                typeof(TException),
                new object[]
                {
                    string.Format("should break! {0}", message)
                }
                );
        }

    }
}
