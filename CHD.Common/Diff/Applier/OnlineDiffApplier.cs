using System;
using CHD.Common.FileSystem;
using CHD.Common.OnlineStatus.Diff.Apply;
using CHD.Common.Sync.Report;

namespace CHD.Common.Diff.Applier
{
    public sealed class OnlineDiffApplier : IDiffApplier
    {
        private readonly IDiffApplier _applier;
        private readonly IDiffApplyOnlineStatus _onlineStatus;

        public OnlineDiffApplier(
            IDiffApplier applier,
            IDiffApplyOnlineStatus onlineStatus
            )
        {
            if (applier == null)
            {
                throw new ArgumentNullException("applier");
            }
            if (onlineStatus == null)
            {
                throw new ArgumentNullException("onlineStatus");
            }
            _applier = applier;
            _onlineStatus = onlineStatus;
        }

        public void Apply(
            IFileSystem local,
            IFileSystem remote,
            IDiff diff
            )
        {
            _onlineStatus.Start(diff);

            _applier.Apply(
                local,
                remote,
                diff
                );
        }

        public SyncReport Commit()
        {
            try
            {
                var result = _applier.Commit();

                return result;
            }
            finally
            {
                _onlineStatus.End();
            }
        }

        public void Revert()
        {
            try
            {
                _applier.Revert();
            }
            finally
            {
                _onlineStatus.End();
            }
        }
    }
}