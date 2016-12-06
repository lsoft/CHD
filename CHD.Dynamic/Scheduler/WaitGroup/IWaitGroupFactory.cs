using CHD.Common;

namespace CHD.Dynamic.Scheduler.WaitGroup
{
    public interface IWaitGroupFactory
    {
        IWaitGroup CreateWaitGroup(
            PerformanceTimer performanceTimer
            );
    }
}