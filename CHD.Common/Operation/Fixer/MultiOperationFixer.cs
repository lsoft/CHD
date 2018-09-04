using System;
using System.Linq;
using System.Threading;

namespace CHD.Common.Operation.Fixer
{
    public sealed class MultiOperationFixer : IOperationFixer
    {
        private readonly IFixer[] _fixers;

        private long _cleanuped = 0L;

        public IOperation Operation
        {
            get;
            private set;
        }

        public MultiOperationFixer(
            IOperation operation,
            params IFixer[] fixers
            )
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }
            if (fixers == null)
            {
                throw new ArgumentNullException("fixers");
            }

            Operation = operation;
            _fixers = fixers;
        }

        public void SafelyCommit()
        {
            var cleanuped = Interlocked.Exchange(ref _cleanuped, 1L);
            if (cleanuped == 0L)
            {
                DoSafelyCommit();
            }
        }

        public void SafelyRevert()
        {
            var cleanuped = Interlocked.Exchange(ref _cleanuped, 1L);
            if (cleanuped == 0L)
            {
                DoSafelyRevert();
            }
        }

        public void Dispose()
        {
            SafelyRevert();
        }

        private void DoSafelyCommit()
        {
            foreach (var fixer in _fixers)
            {
                fixer.SafelyCommit();
            }
        }

        private void DoSafelyRevert()
        {
            foreach (var fixer in _fixers.Reverse())
            {
                fixer.SafelyRevert();
            }
        }
    }
}