using CHD.Common.Diff;
using CHD.Common.OnlineStatus.Diff.Build;
using CHD.Common.Operation;

namespace CHD.Common.OnlineStatus.Diff.Apply
{
    public interface IDiffApplyOnlineStatus
    {
        void Start(
            IDiff diff
            );

        void End(
            );
    }
}