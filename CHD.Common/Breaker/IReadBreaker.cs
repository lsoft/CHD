using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHD.Common.Breaker
{
    public interface IReadBreaker
    {
        bool ShouldBreak
        {
            get;
        }

        string BreakMessage
        {
            get;
        }

        void RaiseExceptionIfBreak(
            string message = ""
            );
    }
}
