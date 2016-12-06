using System;
using CHD.Common;

namespace CHD.Dynamic.Scheduler.WaitGroup.Standard
{
    public class StandardWaitGroupFactory : IWaitGroupFactory
    {
        public IWaitGroup CreateWaitGroup(
            PerformanceTimer performanceTimer
            )
        {
            if (performanceTimer == null)
            {
                throw new ArgumentNullException("performanceTimer");
            }

            return 
                new StandardWaitGroup(
                    performanceTimer
                    );
        }
    }
}
