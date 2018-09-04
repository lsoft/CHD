using System;
using System.Threading;
using CHD.Common;

namespace CHD.Token.Releaser
{
    public sealed class BackgroundReleaser : IDisposable, IBackgroundReleaser
    {
        private readonly IDisorderLogger _logger;

        private int _disposed = 0;

        private readonly ManualResetEvent _stopSignal = new ManualResetEvent(false);
        private Thread _t = null;

        public event BackgroundReleaserChangedDelegate BackgroundReleaserChangedEvent;

        private bool _isReleaseInProgress;
        public bool IsReleaseInProgress
        {
            get
            {
                return
                    _isReleaseInProgress;
            }

            private set
            {
                var isReleaseInProgress = _isReleaseInProgress;
                _isReleaseInProgress = value;

                if (isReleaseInProgress != value)
                {
                    OnBackgroundReleaserChanged();
                }
            }
        }

        public BackgroundReleaser(
            IDisorderLogger logger
            )
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _logger = logger;
        }

        public void SyncRelease(
            Func<bool> releaseTokenFunc
            )
        {
            TryToReleaseAtBackgroundThread(
                releaseTokenFunc
                );

            //wait for token release (= thread death)
            _t.Join();
        }

        public void TryToReleaseAtBackgroundThread(
            Func<bool> releaseTokenFunc
            )
        {
            if (releaseTokenFunc == null)
            {
                throw new ArgumentNullException("releaseTokenFunc");
            }

            if (_t != null && _t.IsAlive)
            {
                throw new InvalidOperationException("Release thread already live.");
            }

            _t = new Thread(DoWork);
            _t.Start(releaseTokenFunc);
        }

        public void Dispose()
        {
            var disposed = Interlocked.Exchange(ref _disposed, 1);

            if (disposed == 0)
            {
                Cleanup();
            }
        }

        private void DoWork(
            object parameter
            )
        {
            var releaseTokenFunc = (Func<bool>)parameter;

            IsReleaseInProgress = true;
            try
            {
                do
                {
                    try
                    {
                        if (releaseTokenFunc())
                        {
                            return;
                        }
                    }
                    catch (Exception excp)
                    {
                        _logger.LogException(excp);
                    }
                }
                while (!_stopSignal.WaitOne(TimeSpan.FromSeconds(1))); //repeat trying to release token at every second
            }
            finally
            {
                IsReleaseInProgress = false;
            }
        }

        private void Cleanup()
        {
            _stopSignal.Set();

            if (_t != null && _t.IsAlive)
            {
                _t.Join();
            }

            try
            {
                _stopSignal.Dispose();
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);
            }
        }

        protected void OnBackgroundReleaserChanged()
        {
            var handler = BackgroundReleaserChangedEvent;
            if (handler != null)
            {
                try
                {
                    handler();
                }
                catch (Exception excp)
                {
                    _logger.LogException(excp);
                }
            }
        }
    }
}
