using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHD.Common
{
    public interface IDisorderLogger
    {
        void LogDateDetailed(DateTime dt, string messageWithMask = "");
        void LogMessage(string message);
        void LogFormattedMessage(string message, params object[] args);
        void LogException(Exception excp, string message = "");
    }
}
