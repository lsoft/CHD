using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHD.Common.Logger;
using CHD.Dynamic.Scheduler.Logger;

namespace CHD.Client.CompositionRoot
{
    internal class SchedulerLoggerProxy : ISchedulerLogger
    {
        private readonly IDisorderLogger _logger;

        public SchedulerLoggerProxy(
            IDisorderLogger logger
            )
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _logger = logger;
        }

        public void LogException(Exception excp)
        {
            _logger.LogException(excp);
        }
    }
}
