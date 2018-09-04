using System;
using CHD.Common.Diff.Conflict;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.OnlineStatus.Diff.Build;

namespace CHD.Common.Diff.Constructor
{
    public sealed class OnlineDiffConstructor : IDiffConstructor
    {
        private readonly IDiffConstructor _diffConstructor;
        private readonly IDiffBuildOnlineStatus _onlineStatus;

        public OnlineDiffConstructor(
            IDiffConstructor diffConstructor,
            IDiffBuildOnlineStatus onlineStatus
            )
        {
            if (diffConstructor == null)
            {
                throw new ArgumentNullException("diffConstructor");
            }
            if (onlineStatus == null)
            {
                throw new ArgumentNullException("onlineStatus");
            }
            _diffConstructor = diffConstructor;
            _onlineStatus = onlineStatus;
        }

        public IDiff BuildDiff(
            IFolder localActual,
            IFolder localLastSynced,
            IFolder remoteActual,
            out IConflictDescription conflictDescription
            )
        {
            IDiff result;
            try
            {
                _onlineStatus.Start();

                result = _diffConstructor.BuildDiff(
                    localActual,
                    localLastSynced,
                    remoteActual,
                    out conflictDescription
                    );
            }
            finally
            {
                _onlineStatus.End();
            }

            return
                result;
        }
    }
}