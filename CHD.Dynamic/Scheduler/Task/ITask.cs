using System;

namespace CHD.Dynamic.Scheduler.Task
{
    public interface ITask
    {
        Guid TaskGuid
        {
            get;
        }

        long MicrosecondsBetweenAwakes
        {
            get;
        }

        void Execute(
            Func<bool> unexpectedBreakFunc,
            out bool needToRepeat
            );
    }
}