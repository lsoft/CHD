using System;

namespace CHD.Common.Breaker
{
    public sealed class Breaker<TException> : IBreaker
        where TException : Exception
    {
        private volatile bool _shouldBreak = false;

        public string BreakMessage
        {
            get;
            private set;
        }

        public bool ShouldBreak
        {
            get
            {
                return
                    _shouldBreak;
            }
        }

        public Breaker(
            )
        {
        }

        public void FireBreak(
            string message = ""
            )
        {
            BreakMessage = message;
            _shouldBreak = true;
        }

        public void ResetBreak()
        {
            BreakMessage = string.Empty;
            _shouldBreak = false;
        }

        public void RaiseExceptionIfBreak(string message = "")
        {
            if (!_shouldBreak)
            {
                return;
            }

            var prefix = string.IsNullOrEmpty(BreakMessage) ? "Unspecified break signal: " : BreakMessage;

            throw (TException)Activator.CreateInstance(
                typeof(TException),
                new object[]
                {
                    string.Format("{0} {1}", prefix, message)
                }
                );
        }
    }
}