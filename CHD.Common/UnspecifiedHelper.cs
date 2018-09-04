using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHD.Common
{
    public static class UnspecifiedHelper
    {
        public static string ToHumanReadableString(
            bool isInProgress,
            bool isCompleted
            )
        {
            if (isCompleted)
            {
                return
                    "Is completed";
            }

            if (isInProgress)
            {
                return
                    "In progress";
            }

            return
                "Idle";
        }
    }
}
