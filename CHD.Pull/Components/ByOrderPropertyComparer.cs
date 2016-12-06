using System.Collections.Generic;
using CHD.Graveyard.RemoteFileState;

namespace CHD.Pull.Components
{
    public class ByOrderPropertyComparer : IComparer<IRemoteFileState>
    {
        public int Compare(IRemoteFileState x, IRemoteFileState y)
        {
            if (x == null && y == null)
            {
                return 0;
            }
            if (x != null && y == null)
            {
                return
                    1;
            }
            if (x == null && y != null)
            {
                return
                    -1;
            }

            return
                x.Order.CompareTo(y.Order);
        }
    }
}