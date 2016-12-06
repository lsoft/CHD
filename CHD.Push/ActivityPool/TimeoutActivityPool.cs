using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using CHD.Ambient;
using CHD.Common.Logger;
using CHD.Dynamic.Scheduler;
using CHD.FileSystem.FileWrapper;
using CHD.Push.Proxy;

namespace CHD.Push.ActivityPool
{
    public class TimeoutActivityPool : ITimeoutActivityPool, IDisposable
    {
        private readonly object _locker = new object();

        private readonly IPool2SchedulerProxy _proxy;
        private readonly TimeSpan _liveTime;
        private readonly TimeSpan _delayTime;
        private readonly IDisorderLogger _logger;

        private readonly Dictionary<IFileWrapper, ActivityContainer> _activityDictionary = new Dictionary<IFileWrapper, ActivityContainer>(new OnlySuffixEqualityComparer());

        private volatile bool _started = false;
        private volatile bool _stopped = false;

        private readonly ManualResetEvent _stop = new ManualResetEvent(false);
        private readonly AutoResetEvent _awake = new AutoResetEvent(false);

        private Thread _workThread;

        public TimeoutActivityPool(
            IPool2SchedulerProxy proxy,
            TimeSpan liveTime,
            TimeSpan delayTime,
            IDisorderLogger logger
            )
        {
            if (proxy == null)
            {
                throw new ArgumentNullException("proxy");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (liveTime <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("liveTime");
            }
            if (delayTime <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("delayTime");
            }
            if (liveTime <= delayTime)
            {
                throw new ArgumentException("liveTime <= delayTime");
            }

            _proxy = proxy;
            _liveTime = liveTime;
            _delayTime = delayTime;
            _logger = logger;
        }

        public void ApplyActivity(
            ActivityTypeEnum activity,
            IFileWrapper fileWrapper
            )
        {
            lock (_locker)
            {
                //stop ongoing operations with this file
                _proxy.CancelTask(fileWrapper);

                switch (activity)
                {
                    case ActivityTypeEnum.CreateOrChange:
                        ProcessCreateOrChange(fileWrapper);
                        break;
                    case ActivityTypeEnum.Delete:
                        ProcessDelete(fileWrapper);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            //awake a work thread for refreshing
            _awake.Set();
        }

        public void AsyncStart()
        {
            if (_started)
            {
                return;
            }

            _started = true;

            _workThread = new Thread(DoWork);
            _workThread.Start();
        }

        public void SyncStop()
        {
            if (_stopped)
            {
                return;
            }

            _stopped = true;

            Stop();
            Cleanup();
        }

        public void Dispose()
        {
            SyncStop();
        }

        #region background queue

        private void DoWork()
        {
            while (true)
            {
                try
                {
                    //taking the closest fileWrapper
                    IFileWrapper closestFileWrapper;
                    ActivityContainer closestActivity;
                    GetClosestActivity(
                        out closestFileWrapper,
                        out closestActivity
                        );

                    //and calculating wait timeout
                    TimeSpan waitTimeout;
                    if (closestActivity != null)
                    {
                        var now = AmbientContext.DateTimeProvider.GetCurrentTime();
                        waitTimeout = closestActivity.FireDateTime - now;

                        //it's possibly to be closest event lay in the past
                        if (waitTimeout < TimeSpan.Zero)
                        {
                            waitTimeout = TimeSpan.Zero;
                        }
                    }
                    else
                    {
                        //no changes, wait for infinite
                        waitTimeout = TimeSpan.FromMilliseconds(-1);
                    }

                    //waiting
                    var i = WaitHandle.WaitAny(
                        new WaitHandle[]
                        {
                            _stop,
                            _awake
                        },
                        waitTimeout
                        );

                    //process wait results
                    switch (i)
                    {
                        case 0:
                            //stop fired!
                            //we should stop, but first we should fire all stored activities
                            FireAllActivities();
                            return;

                        case 1:
                            //awake fired!
                            //we should just repeat
                            break;

                        case WaitHandle.WaitTimeout:
                            //timeout!
                            //we should fire the fileWrapper
                            FireActivity(
                                closestFileWrapper,
                                closestActivity
                                );
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(i.ToString(CultureInfo.InvariantCulture));
                    }
                }
                catch (Exception excp)
                {
                    _logger.LogException(excp);

                    //in case of exception we need to restart
                    //without modifying our fileWrapper container
                }
            }
        }

        private void FireAllActivities()
        {
            lock (_locker)
            {
                foreach (var c in _activityDictionary)
                {
                    try
                    {
                        FireActivity(
                            c.Key,
                            c.Value
                            );
                    }
                    catch (Exception excp)
                    {
                        _logger.LogException(excp);
                    }
                }
            }
        }

        private void FireActivity(
            IFileWrapper fileWrapper, 
            ActivityContainer activity
            )
        {
            lock (_locker)
            {
                ActivityContainer existingActivity;
                if (_activityDictionary.TryGetValue(fileWrapper, out existingActivity))
                {
                    if (Equals(existingActivity, activity)) //should here write directly ReferenceEquals?
                    {
                        //nothing changes in the container

                        try
                        {
                            //remove obsolete data
                            _activityDictionary.Remove(fileWrapper);

                            //fire the fileWrapper (in sync mode! under the lock!)
                            _proxy.AddTask(
                                activity.Activity,
                                fileWrapper
                                );

                            //everything is done!
                        }
                        catch
                        {
                            //in case of error we must delay a firing the fileWrapper
                            //because it may occurs in case if source file is locked by newest changer

                            activity.DelayFiring(_delayTime);

                            throw;
                        }
                    }
                    else
                    {
                        //processed fileWrapper has been removed from the container
                        //it's obsolete fileWrapper
                        //nothing should be performed
                    }
                }
                else
                {
                    //processed fileWrapper has been removed from the container
                    //it's obsolete fileWrapper
                    //nothing should be performed
                }
            }
        }

        private void GetClosestActivity(
            out IFileWrapper closestFileWrapper,
            out ActivityContainer closestActivity
            )
        {
            closestFileWrapper = null;
            closestActivity = null;

            lock (_locker)
            {
                if (_activityDictionary.Count > 0)
                {
                    var pair = (
                        from p in _activityDictionary
                        orderby p.Value.FireDateTime ascending
                        select p
                        ).First();

                    closestFileWrapper = pair.Key;
                    closestActivity = pair.Value;
                }
            }
        }

        #endregion

        #region stop and cleanup

        private void Stop()
        {
            _stop.Set();

            if (_workThread != null)
            {
                _workThread.Join();
            }
        }

        private void Cleanup()
        {
            try
            {
                _stop.Dispose();
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);
            }

            try
            {
                _awake.Dispose();
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);
            }
        }
        

        #endregion

        #region process activities

        private void ProcessCreateOrChange(IFileWrapper fileWrapper)
        {
            var activity = new ActivityContainer(
                ActivityTypeEnum.CreateOrChange,
                AmbientContext.DateTimeProvider.GetCurrentTime() + _liveTime
                );

            ProcessActivity2(activity, fileWrapper);
        }

        private void ProcessDelete(IFileWrapper fileWrapper)
        {
            var activity = new ActivityContainer(
                ActivityTypeEnum.Delete,
                AmbientContext.DateTimeProvider.GetCurrentTime() + _liveTime
                );

            ProcessActivity2(activity, fileWrapper);
        }

        private void ProcessActivity2(
            ActivityContainer activity,
            IFileWrapper file
            )
        {
            //этот метод в принципе, выродился в простую замену ченжа на новый ченж
            //без разницы от вида старого ченжа и вида нового ченжа
            //но я не буду упрощать этот метод, так как в тексте написаны комментарии,
            //содержащие мотивацию замены ченжа для каждого случая

            ActivityContainer currentActivity;
            if (_activityDictionary.TryGetValue(file, out currentActivity))
            {
                //что-то с этим файлом уже делали

                switch (activity.Activity)
                {
                    case ActivityTypeEnum.CreateOrChange:
                        //файл создан, но при этом какая-то операция с этим файлом уже есть

                        switch (currentActivity.Activity)
                        {
                            case ActivityTypeEnum.CreateOrChange:
                                //если предыдущая операция - создания или изменения, то удаляем старую операцию
                                //и заменяем ее новой, чтобы метка времени обновилась
                                _activityDictionary[file] = activity;
                                break;
                            case ActivityTypeEnum.Delete:
                                //если предыдущая операция удаления (а эта - изменения), то будем считать, что файл был пересоздан
                                //заменяем операцию удаления на операцию создания или изменения
                                _activityDictionary[file] = activity;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                    case ActivityTypeEnum.Delete:
                        //файл удален, но операция с ним еще есть
                        //сохраненная операция не нужна, заменяем на удаление
                        _activityDictionary[file] = activity;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("fileWrapper.activity");
                }
            }
            else
            {
                //с этим файлом ничего не делали
                //просто добавляем изменение и всё

                _activityDictionary.Add(
                    file,
                    activity
                    );
            }
        }

        #endregion

        #region private classes

        private class ActivityContainer
        {
            public ActivityTypeEnum Activity
            {
                get;
                private set;
            }

            public DateTime FireDateTime
            {
                get;
                private set;
            }

            public ActivityContainer(
                ActivityTypeEnum activity,
                DateTime fireDateTime
                )
            {
                Activity = activity;
                FireDateTime = fireDateTime;
            }

            public void DelayFiring(TimeSpan delayTime)
            {
                FireDateTime += delayTime;
            }

            #region equality

            protected bool Equals(ActivityContainer other)
            {
                return Activity == other.Activity && FireDateTime.Equals(other.FireDateTime);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                if (obj.GetType() != this.GetType())
                {
                    return false;
                }
                return Equals((ActivityContainer) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((int) Activity*397) ^ FireDateTime.GetHashCode();
                }
            }

            #endregion
        }

        #endregion

    }
}
