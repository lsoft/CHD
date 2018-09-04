using System;

namespace CHD.Common.Sync.Report
{
    public static class SyncResultEnumHelper
    {
        public static string ToHumanReadableString(
            this SyncResultEnum syncResult
            )
        {
            switch (syncResult)
            {
                case SyncResultEnum.AlreadyPerforming:
                    return "Already performing";
                case SyncResultEnum.NoChangesDetected:
                    return "No changes detected";
                case SyncResultEnum.Completed:
                    return "Completed";
                default:
                    throw new ArgumentOutOfRangeException(syncResult.ToString());
            }
        }        
    }
}