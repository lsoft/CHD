using System;
using System.Collections.Generic;
using System.Linq;
using CHD.Common;
using CHD.Dynamic.Scheduler.Event;
using CHD.Dynamic.Scheduler.Logger;
using CHD.Dynamic.Scheduler.SchedulerThread;
using CHD.Dynamic.Scheduler.SchedulerThread.Factory;
using CHD.Dynamic.Scheduler.Task;
using CHD.Dynamic.Scheduler.WaitGroup;

namespace CHD.Dynamic.Scheduler
{
    public class Scheduler : IScheduler, IDisposable
    {
        private readonly ISchedulerLogger _logger;
        private readonly IWaitGroup _waitGroup;
        private readonly IThread _thread;

        private readonly TaskContainer _taskContainer;
        private readonly PerformanceTimer _timer;

        private volatile bool _started = false;
        private volatile bool _stopped = false;

        public event SchedulerEventDelegate SchedulerEvent;

        public int TaskCount
        {
            get
            {
                return
                    _taskContainer.TaskCount;
            }
        }

        public Scheduler(
            IWaitGroupFactory waitGroupFactory,
            IThreadFactory threadFactory,
            ISchedulerLogger logger
            )
        {
            if (waitGroupFactory == null)
            {
                throw new ArgumentNullException("waitGroupFactory");
            }
            if (threadFactory == null)
            {
                throw new ArgumentNullException("threadFactory");
            }

            _logger = logger;

            _taskContainer = new TaskContainer();
            _timer = new PerformanceTimer();

            _waitGroup = waitGroupFactory.CreateWaitGroup(
                _timer
                );

            _thread = threadFactory.CreateThread(
                DoWork
                );
        }

        public void Start()
        {
            if (_stopped)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
            if (_started)
            {
                return;
            }

            _timer.Restart();

            //����������� �������� _started �������� ����� �������� �������, 
            //��� ��� � ���������� ����� ���� ������, ���������� �� _started � _timer

            //��� ��� _timer.Restart ��� �����, ��������� � ������� ��������, � _started = true ��� ����
            //������ ��������, �� ��� �������� �� ����� ���� ��������������� (CLR ��������� ����� �����������)
            //��� ��� ������������� � ������� ������

            _started = true;

            _thread.Start();

            //������ �������
            OnStarted();
        }

        public void Stop()
        {
            DoStop();
        }

        public void AddTask(
            ITask task
            )
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            //����������� ��������� ������ � �����������, ���� �� �� ���������
            if (_stopped)
            {
                return;
            }

            ISchedulerTask schedulerTask;

            //���� ����������� ��� ���������, �� ���� ��������������� ����� ����������� �� �������� ��� �������������� �������
            //��� ����������, ��� ��� � Task �������� ������ �����������, � �� ����������������� ���
            if (_started)
            {
                schedulerTask = new SchedulerTask(
                    task,
                    _timer,
                    true
                    );
            }
            else
            {
                schedulerTask = new SchedulerTask(
                    task,
                    _timer,
                    false
                    );
            }

            _taskContainer.AddTask(schedulerTask);

            _waitGroup.RaiseEvent(WaitGroupEventEnum.Restart);
        }

        public void CancelTask(
            Guid taskGuid
            )
        {
            //����������� �������� ������ � ������������, ���� �� �� ���������
            if (_stopped)
            {
                return;
            }
            
            _taskContainer.RemoveTask(taskGuid);

            _waitGroup.RaiseEvent(WaitGroupEventEnum.Restart);
        }

        public void Dispose()
        {
            DoStop();
        }

        private void DoStop()
        {
            if (!_stopped)
            {
                _stopped = true;

                //������ ������� ������ ���������
                OnStopping();
                try
                {
                    _waitGroup.RaiseEvent(WaitGroupEventEnum.Stop);
                    _thread.Join();

                    _waitGroup.Dispose();
                }
                finally
                {
                    OnUncompletedTasks();

                    //������ ������� ���������� ���������
                    OnStopped();
                }
            }
        }

        private void OnUncompletedTasks()
        {
            while (true)
            {
                var schedulerTask = _taskContainer.GetClosest();

                if (schedulerTask == null)
                {
                    return;
                }

                var task = schedulerTask.Task;
                if (task != null)
                {
                    OnUncompletedTask(task);
                }

                _taskContainer.RemoveTask(schedulerTask);
            }
        }

        private void DoWork()
        {
            try
            {
                while (true)
                {
                    //��������� ������ (����� "close" - �� ��������, � ���������)
                    var closedTask = _taskContainer.GetClosest();

                    long microsecondsToAwake; //������ (� �������������) � ������� ��� ������ ���� ���������
                    if (closedTask == null)
                    {
                        //���� ������ ��� - ���� ������������� ����� �� �������� _stopEvent ��� _restartEvent
                        microsecondsToAwake = -1;

                        //������ ������� ��� �����
                        OnNoTask();
                    }
                    else
                    {
                        //���������� ������� ����� ��������� ������
                        microsecondsToAwake = closedTask.MicrosecondsToAwake;

                        if (microsecondsToAwake < 0)
                        {
                            //�������� ������������� - ��� �������� ����� ��������!
                            //������ - �� ���� ������
                            microsecondsToAwake = 0L;
                        }
                    }

                    var waitResult = _waitGroup.WaitAny(microsecondsToAwake);

                    switch (waitResult)
                    {
                        case WaitGroupEventEnum.Stop:
                            //��������� �����������
                            return;

                        case WaitGroupEventEnum.Restart:
                            //���� ���������� � ��������� ����� ���������� ������
                            //������ ����� ������ ��� ����� ��� �������� ��� ������
                            break;

                        case WaitGroupEventEnum.WaitTimeout:
                            //������� ���������
                            if (closedTask != null)
                            {
                                var needToRepeat = false;
                                try
                                {
                                    //������ ������� ������ ���������� �����
                                    OnTaskBeginExecution(closedTask.TaskGuid);

                                    //�������� �����, ��������, ��� ���� ���� "���������"
                                    closedTask.Execute(
                                        () => _waitGroup.WaitAny(0L) == WaitGroupEventEnum.Stop,
                                        out needToRepeat
                                        );
                                }
                                catch (Exception excp)
                                {
                                    //��������� ������ ���������� ������
                                    //�� ������ ���, � �������� �����. �����
                                    //���������� ������ ��� ���������, ���� �� ������ ��������� � ���
                                    OnTaskRaisedException(excp);
                                }
                                finally
                                {
                                    //������ ������� ���������� ���������� �����
                                    OnTaskEndExecution(closedTask.TaskGuid);

                                    if (needToRepeat)
                                    {
                                        //������ ���� ���������, � ��� ���������� ���� (�������������� ������) ���������� �������
                                        //���� �������� ��� ������ � ����������, ����� ��������� ��������
                                        _taskContainer.Refresh(closedTask);
                                    }
                                    else
                                    {
                                        //������� ������ �� ��������
                                        //� ������ ������ ����� �� ����� ������� catch
                                        //��� ��������� OnCriticalException � ������� ����������� 
                                        _taskContainer.RemoveTask(closedTask);
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception excp)
            {
                //����������� ������ ������ ��������
                //����� ������� ����������� ������� �������� ��� ��������� ������� ��������
                OnCriticalException(excp);
            }
        }

        #region event related code

        private void OnStarted()
        {
            OnScheduler(
                new SchedulerEventDescription(
                    SchedulerEventTypeEnum.Started
                    )
                );
        }

        private void OnStopping()
        {
            OnScheduler(
                new SchedulerEventDescription(
                    SchedulerEventTypeEnum.Stopping
                    )
                );
        }

        private void OnStopped()
        {
            OnScheduler(
                new SchedulerEventDescription(
                    SchedulerEventTypeEnum.Stopped
                    )
                );
        }

        private void OnTaskRaisedException(
            Exception excp
            )
        {
            if (excp == null)
            {
                throw new ArgumentNullException("excp");
            }

            var result = OnScheduler(
                new SchedulerEventDescription(
                    SchedulerEventTypeEnum.TaskRaisedException,
                    excp
                    )
                );

            if (result == EventHandlerResultEnum.NoSubscribers)
            {
                //� ����� ������ ����������� ���
                //������� ������ ������� � ���
                _logger.LogException(excp);
            }
        }

        private void OnNoTask()
        {
            OnScheduler(
                new SchedulerEventDescription(
                    SchedulerEventTypeEnum.NoTask
                    )
                );
        }

        private void OnTaskBeginExecution(
            Guid taskGuid
            )
        {
            OnScheduler(
                new SchedulerEventDescription(
                    SchedulerEventTypeEnum.TaskBeginExecution,
                    taskGuid
                    )
                );
        }

        private void OnTaskEndExecution(
            Guid taskGuid
            )
        {
            OnScheduler(
                new SchedulerEventDescription(
                    SchedulerEventTypeEnum.TaskEndExecution,
                    taskGuid
                    )
                );
        }

        private void OnCriticalException(
            Exception excp
            )
        {
            if (excp == null)
            {
                throw new ArgumentNullException("excp");
            }

            var result = OnScheduler(
                new SchedulerEventDescription(
                    SchedulerEventTypeEnum.CriticalException,
                    excp
                    )
                );

            if (result == EventHandlerResultEnum.NoSubscribers)
            {
                //� ����� ������ ����������� ���
                //������� ������ ������� � ���
                _logger.LogException(excp);
            }
        }

        private EventHandlerResultEnum OnScheduler(
            SchedulerEventDescription argument
            )
        {
            if (argument == null)
            {
                throw new ArgumentNullException("argument");
            }

            var result = EventHandlerResultEnum.Success;

            SchedulerEventDelegate handler = SchedulerEvent;
            if (handler != null)
            {
                try
                {
                    handler(argument);
                }
                catch (Exception excp)
                {
                    //���� � ���������� ��������� ����, �� ������ ����� �������� ��������� � �������� TaskRaisedException
                    //��� ��� ��� ����� ���� ������ ����
                    //������� ������ ����� � ���

                    _logger.LogException(excp);

                    result = EventHandlerResultEnum.HandleException;
                }
            }
            else
            {
                result = EventHandlerResultEnum.NoSubscribers;
            }

            return
                result;
        }

        private void OnUncompletedTask(ITask task)
        {
            OnScheduler(
                new SchedulerEventDescription(
                    SchedulerEventTypeEnum.TaskUncomplete,
                    task
                    )
                );
        }

        private enum EventHandlerResultEnum
        {
            /// <summary>
            /// ��� �����������
            /// </summary>
            NoSubscribers,

            /// <summary>
            /// ������ � �������� ��������� ������
            /// </summary>
            HandleException,

            /// <summary>
            /// ������� ���������� �������
            /// </summary>
            Success
        }

        #endregion

        #region support classes

        private class TaskContainer
        {
            private readonly SortedSet<ISchedulerTask> _set = new SortedSet<ISchedulerTask>(new TaskComparer());
            private readonly Dictionary<Guid, ISchedulerTask> _dict = new Dictionary<Guid, ISchedulerTask>();

            private readonly object _locker = new object();

            public int TaskCount
            {
                get
                {
                    lock (_locker)
                    {
                        return
                            _set.Count;
                    }
                }
            }

            public void AddTask(
                ISchedulerTask task
                )
            {
                if (task == null)
                {
                    throw new ArgumentNullException("task");
                }

                lock (_locker)
                {
                    _set.Add(task);
                    _dict.Add(task.TaskGuid, task);
                }
            }

            public ISchedulerTask GetClosest()
            {
                ISchedulerTask result;

                lock (_locker)
                {
                    result = _set.FirstOrDefault();
                }
                
                return
                    result;
            }

            public void RemoveTask(
                ISchedulerTask task
                )
            {
                if (task == null)
                {
                    throw new ArgumentNullException("task");
                }

                lock (_locker)
                {
                    _set.Remove(task);
                    _dict.Remove(task.TaskGuid);
                }
            }

            public void RemoveTask(
                Guid taskGuid
                )
            {
                lock (_locker)
                {
                    ISchedulerTask task;
                    if (_dict.TryGetValue(taskGuid, out task))
                    {
                        _set.Remove(task);
                        _dict.Remove(taskGuid);
                    }
                }
            }

            public void Refresh(
                ISchedulerTask task
                )
            {
                if (task == null)
                {
                    throw new ArgumentNullException("task");
                }

                lock (_locker)
                {
                    _set.Remove(task);
                    _dict.Remove(task.TaskGuid);

                    _set.Add(task);
                    _dict.Add(task.TaskGuid, task);
                }
            }

            public class TaskComparer : IComparer<ISchedulerTask>
            {
                public int Compare(ISchedulerTask x, ISchedulerTask y)
                {
                    if (x == null)
                    {
                        throw new ArgumentNullException("x");
                    }
                    if (y == null)
                    {
                        throw new ArgumentNullException("y");
                    }

                    return
                        x.MicrosecondsToAwake.CompareTo(y.MicrosecondsToAwake);
                }

            }
        }

        public interface ISchedulerTask
        {
            Guid TaskGuid
            {
                get;
            }

            long MicrosecondsToAwake
            {
                get;
            }

            ITask Task
            {
                get;
            }

            void Execute(
                Func<bool> unexpectedBreakFunc,
                out bool needToRepeat
                );
        }

        private class SchedulerTask : ISchedulerTask
        {
            private readonly PerformanceTimer _timer;
            private long _preTime;

            public Guid TaskGuid
            {
                get
                {
                    return
                        Task.TaskGuid;
                }
            }

            public long MicrosecondsToAwake
            {
                get
                {
                    var r = _preTime;

                    r += Task.MicrosecondsBetweenAwakes;

                    return
                        r;
                }
            }

            public ITask Task
            {
                get;
                private set;
            }

            public SchedulerTask(
                ITask task,
                PerformanceTimer timer,
                bool needToCorrect
                )
            {
                if (task == null)
                {
                    throw new ArgumentNullException("task");
                }
                if (timer == null)
                {
                    throw new ArgumentNullException("timer");
                }

                Task = task;

                _timer = timer;

                if (needToCorrect)
                {
                    //����������� ��� ���������
                    //���� ��������������� ����� ����������� �� �������� ��� �������������� �������
                    //��� ����������, ��� ��� � SchedulerTask �������� ������ �����������, � �� ����������������� ���
                    _preTime = timer.MicroSeconds;
                }
                else
                {
                    //����������� �� ���������
                    //�������������� ����� �� ����
                    _preTime = 0L;
                }
            }

            public void Execute(
                Func<bool> unexpectedBreakFunc,
                out bool needToRepeat
                )
            {
                needToRepeat = false;
                try
                {
                    Task.Execute(
                        unexpectedBreakFunc,
                        out needToRepeat
                        );
                }
                finally
                {
                    if (needToRepeat)
                    {
                        //���� ��������� ������
                        //����� ���� ���� ������� ���������� �������

                        _preTime = _timer.MicroSeconds;
                    }
                }
            }
        }

        #endregion
    }
}