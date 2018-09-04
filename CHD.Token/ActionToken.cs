using System;
using System.Threading;

namespace CHD.Token
{
    public sealed class ActionToken : IToken
    {
        private readonly Action _freeToken;

        private int _disposed = 0;

        public ActionToken(
            Action freeToken
            )
        {
            if (freeToken == null)
            {
                throw new ArgumentNullException("freeToken");
            }

            _freeToken = freeToken;
        }

        public void Dispose()
        {
            var disposed = Interlocked.Exchange(ref _disposed, 1);

            if (disposed == 0)
            {
                _freeToken();
            }
        }


    }
}