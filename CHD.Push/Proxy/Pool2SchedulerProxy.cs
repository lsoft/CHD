using System;
using CHD.Dynamic.Scheduler;
using CHD.FileSystem.FileWrapper;
using CHD.Push.ActivityPool;
using CHD.Push.Task.Factory;
using CHD.Push.Task.GuidProvider;

namespace CHD.Push.Proxy
{
    public class Pool2SchedulerProxy : IPool2SchedulerProxy
    {
        private readonly IAlgorithmGuidProvider _guidProvider;
        private readonly IScheduler _scheduler;
        private readonly IAlgorithmFactory _algorithmFactory;
        private readonly int _pushTimeoutAfterFailureMsec;

        public Pool2SchedulerProxy(
            IAlgorithmGuidProvider guidProvider,
            IScheduler scheduler,
            IAlgorithmFactory algorithmFactory,
            int pushTimeoutAfterFailureMsec
            )
        {
            if (guidProvider == null)
            {
                throw new ArgumentNullException("guidProvider");
            }
            if (scheduler == null)
            {
                throw new ArgumentNullException("scheduler");
            }
            if (algorithmFactory == null)
            {
                throw new ArgumentNullException("algorithmFactory");
            }
            _guidProvider = guidProvider;
            _scheduler = scheduler;
            _algorithmFactory = algorithmFactory;
            _pushTimeoutAfterFailureMsec = pushTimeoutAfterFailureMsec;
        }

        public void AddTask(
            ActivityTypeEnum activity, 
            IFileWrapper fileWrapper
            )
        {
            if (fileWrapper == null)
            {
                throw new ArgumentNullException("fileWrapper");
            }

            var taskGuid = _guidProvider.GenerateGuid(
                fileWrapper
                );

            var algorithm = _algorithmFactory.Create(
                taskGuid,
                activity,
                fileWrapper,
                _pushTimeoutAfterFailureMsec
                );

            _scheduler.AddTask(
                algorithm
                );
        }

        public void CancelTask(
            IFileWrapper fileWrapper
            )
        {
            if (fileWrapper == null)
            {
                throw new ArgumentNullException("fileWrapper");
            }

            var taskGuid = _guidProvider.GenerateGuid(
                fileWrapper
                );

            _scheduler.CancelTask(
                taskGuid
                );
        }
    }
}
