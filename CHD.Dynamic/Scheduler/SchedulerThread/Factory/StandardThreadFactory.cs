using System.Threading;

namespace CHD.Dynamic.Scheduler.SchedulerThread.Factory
{
    public class StandardThreadFactory : IThreadFactory
    {
        public IThread CreateThread(ThreadStart threadStart)
        {
            var thread = new Thread(threadStart);

            return 
                new ThreadWrapper(thread);
        }
    }
}
