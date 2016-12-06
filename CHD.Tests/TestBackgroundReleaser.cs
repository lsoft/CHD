using System;
using System.Diagnostics;
using System.Threading;
using CHD.Graveyard.Token.Releaser;

namespace CHD.Tests
{
    internal class TestBackgroundReleaser : IBackgroundReleaser
    {
        public TestBackgroundReleaser(
            )
        {
        }

        public event BackgroundReleaserChangedDelegate BackgroundReleaserChangedEvent;

        public bool IsReleaseInProgress
        {
            get;
            private set;
        }

        public void SyncRelease(Func<bool> releaseTokenFunc)
        {
            DoRelease(releaseTokenFunc);
        }

        public void TryToReleaseAtBackgroundThread(Func<bool> releaseTokenFunc)
        {
            DoRelease(releaseTokenFunc);
        }

        private void DoRelease(Func<bool> releaseTokenFunc)
        {
            var result = false;

            if (releaseTokenFunc != null)
            {
                IsReleaseInProgress = true;
                OnBackgroundReleaserChanged();
                try
                {
                    result = releaseTokenFunc();
                }
                finally
                {
                    IsReleaseInProgress = false;
                    OnBackgroundReleaserChanged();
                }
            }

            Debug.WriteLine(
                "Thread: {0}, token released: {1}",
                Thread.CurrentThread.ManagedThreadId,
                result
                );
        }

        protected virtual void OnBackgroundReleaserChanged()
        {
            var handler = BackgroundReleaserChangedEvent;
            if (handler != null)
            {
                try
                {
                    handler();
                }
                catch
                {
                    //nothing to do
                }
            }
        }

    }
}