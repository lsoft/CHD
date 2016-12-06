using System;
using CHD.Common.Logger;
using CHD.Dynamic.Scheduler;
using CHD.Dynamic.Scheduler.Event;
using CHD.Dynamic.Scheduler.Task;
using CHD.Push.Task.Store;

namespace CHD.Push.Task.SaverLoader
{
    public class SaverLoaderScheduler : IScheduler
    {
        private readonly IScheduler _scheduler;
        private readonly IAlgorithmPermanentStore _permanentStore;
        private readonly IDisorderLogger _logger;

        public SaverLoaderScheduler(
            IScheduler scheduler,
            IAlgorithmPermanentStore permanentStore,
            IDisorderLogger logger
            )
        {
            if (scheduler == null)
            {
                throw new ArgumentNullException("scheduler");
            }
            if (permanentStore == null)
            {
                throw new ArgumentNullException("permanentStore");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _scheduler = scheduler;
            _permanentStore = permanentStore;
            _logger = logger;
        }

        private void SaveTask(SchedulerEventDescription argument)
        {
            if (argument != null)
            {
                if (argument.SchedulerEventType == SchedulerEventTypeEnum.TaskUncomplete)
                {
                    var algorithm = argument.AdditionalInformation as IAlgorithm;
                    if (algorithm != null)
                    {
                        Save(algorithm);

                        _logger.LogFormattedMessage(
                            "Algorithm {0} with file {1} has been stored to a permanent store.",
                            algorithm.Type,
                            algorithm.Pusher.FileWrapper.FilePathSuffix
                            );
                    }
                }
            }
        }

        private void Save(
            IAlgorithm algorithm
            )
        {
            if (algorithm == null)
            {
                throw new ArgumentNullException("algorithm");
            }

            _permanentStore.Save(
                algorithm
                );
        }

        private void LoadTasks()
        {
            var delayedTasks = _permanentStore.Load();
            foreach (var task in delayedTasks)
            {
                _scheduler.AddTask(task);

                _logger.LogFormattedMessage(
                    "Algorithm {0} with file {1} has been loaded from a permanent store.",
                    task.Type,
                    task.Pusher.FileWrapper.FilePathSuffix
                    );
            }
        }

        #region Implementation of IScheduler

        public event SchedulerEventDelegate SchedulerEvent
        {
            add
            {
                _scheduler.SchedulerEvent += value;
            }
            remove
            {
                _scheduler.SchedulerEvent -= value;
            }
        }

        public int TaskCount
        {
            get
            {
                return
                    _scheduler.TaskCount;
            }
        }

        public void Start()
        {
            //загружаем сохраненные задачи
            LoadTasks();

            _scheduler.Start();
        }

        public void Stop()
        {
            try
            {
                _scheduler.SchedulerEvent += SaveTask;

                _scheduler.Stop();
            }
            finally
            {
                _scheduler.SchedulerEvent -= SaveTask;
            }
        }

        public void AddTask(ITask task)
        {
            _scheduler.AddTask(task);
        }

        public void CancelTask(Guid taskGuid)
        {
            _scheduler.CancelTask(taskGuid);
        }

        #endregion

    }
}
