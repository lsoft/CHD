using System.Threading;

namespace CHD.Dynamic.Scheduler.SchedulerThread.Factory
{
    public interface IThreadFactory
    {
        IThread CreateThread(ThreadStart threadStart);
    }
}