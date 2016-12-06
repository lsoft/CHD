using System;
using System.Threading;
using CHD.Graveyard.Token;
using CHD.Graveyard.Token.Factory;

namespace CHD.Local.Token
{
    public class LocalTokenController : ITokenController, IDisposable
    {
        private const string SemaphoreName = @"Global\CHD.Semaphore.Local";

        private readonly Semaphore _semaphore;

        private int _disposed = 0;

        public LocalTokenController(
            )
        {
            _semaphore = new Semaphore(1, 1, SemaphoreName);
        }

        public bool TryToObtainToken(
            out IToken token
            )
        {
            if (!_semaphore.WaitOne(TimeSpan.Zero))
            {
                token = null;

                return
                    false;
            }

            token = new ActionToken(
                SafelyReleaseSemaphore
                );

            return
                true;
        }

        public bool TryToReleaseToken()
        {
            SafelyReleaseSemaphore();

            return
                true;
        }

        public void Dispose()
        {
            var disposed = Interlocked.Exchange(ref _disposed, 1);
            if (disposed == 0)
            {
                DoCleanup();
            }
        }

        private void DoCleanup()
        {
            SafelyReleaseSemaphore();

            _semaphore.Close();
        }

        private void SafelyReleaseSemaphore()
        {
            try
            {
                _semaphore.Release();
            }
            catch (SemaphoreFullException)
            {
                //nothing should performed here
            }
        }
    }
}