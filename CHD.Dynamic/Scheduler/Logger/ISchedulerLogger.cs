using System;

namespace CHD.Dynamic.Scheduler.Logger
{
    public interface ISchedulerLogger
    {
        void LogException(Exception excp);
    }
}
