using System.Collections;
using System.Globalization;
using CHD.Common.Diff.Conflict;
using CHD.Common.FileSystem;
//using CHD.Common.KeyValueContainer.SyncedVersion;
using CHD.Common.FileSystem.FFolder;

namespace CHD.Common.Diff.Constructor
{
    public interface IDiffConstructor
    {
        IDiff BuildDiff(
            IFolder localActual,
            IFolder localLastSynced,
            IFolder remoteActual,
            out IConflictDescription conflictDescription
            );

    }
}