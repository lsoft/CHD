using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerformanceTelemetry;

namespace CHD.Service.CompositionRoot.Fakes
{
    public sealed class FakeTelemetryLogger : ITelemetryLogger
    {
        public void LogMessage(Type sourceType, string message)
        {
            //nothing to do
        }

        public void LogHandledException(Type sourceType, string message, Exception excp)
        {
            //nothing to do
        }
    }
}
