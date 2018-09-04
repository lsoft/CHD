using System;
using System.Threading;

namespace CHD.Common.Operation.Fixer
{
    public sealed class ActionFixer<T> : IFixer<T>
    {
        private readonly Action _safelyCommit;
        private readonly Action _safelyRevert;

        private long _cleanuped = 0L;

        public T Result
        {
            get;
            private set;
        }

        public ActionFixer(
            T result,
            Action safelyCommit,
            Action safelyRevert
            )
        {
            if (safelyCommit == null)
            {
                throw new ArgumentNullException("safelyCommit");
            }
            if (safelyRevert == null)
            {
                throw new ArgumentNullException("safelyRevert");
            }

            Result = result;

            _safelyCommit = safelyCommit;
            _safelyRevert = safelyRevert;
        }

        public void SafelyCommit()
        {
            var cleanuped = Interlocked.Exchange(ref _cleanuped, 1L);
            if (cleanuped == 0L)
            {
                _safelyCommit();
            }
        }

        public void SafelyRevert()
        {
            var cleanuped = Interlocked.Exchange(ref _cleanuped, 1L);
            if (cleanuped == 0L)
            {
                _safelyRevert();
            }
        }

        public void Dispose()
        {
            SafelyRevert();
        }

    }
}