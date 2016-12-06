using System;

namespace CHD.Graveyard.Token.Releaser
{
    public interface IBackgroundReleaser
    {
        event BackgroundReleaserChangedDelegate BackgroundReleaserChangedEvent;

        bool IsReleaseInProgress
        {
            get;
        }

        void SyncRelease(
            Func<bool> releaseTokenFunc
            );

        void TryToReleaseAtBackgroundThread(
            Func<bool> releaseTokenFunc
            );
    }
}