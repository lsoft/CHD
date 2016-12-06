using System;
using System.Runtime.Serialization;
using System.Xml;
using CHD.Common.Logger;
using CHD.Push.ActivityPool;
using CHD.Push.Pusher;

namespace CHD.Push.Task
{
    internal abstract class BaseAlgorithm : IAlgorithm
    {
        private readonly object _locker = new object();

        protected readonly IPusher _pusher;
        private readonly int _pushTimeoutAfterFailureMsec;
        protected readonly IDisorderLogger _logger;

        public ActivityTypeEnum Type
        {
            get
            {
                return
                    _pusher.Type;
            }
        }

        public IPusher Pusher
        {
            get
            {
                return
                    _pusher;
            }
        }

        public Guid TaskGuid
        {
            get;
            private set;
        }

        public long MicrosecondsBetweenAwakes
        {
            get
            {
                return
                    _pushTimeoutAfterFailureMsec * 1000L;
            }
        }


        protected BaseAlgorithm(
            IPusher pusher,
            Guid taskGuid,
            int pushTimeoutAfterFailureMsec,
            IDisorderLogger logger
            )
        {
            if (pusher == null)
            {
                throw new ArgumentNullException("pusher");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _pusher = pusher;
            _pushTimeoutAfterFailureMsec = pushTimeoutAfterFailureMsec;
            _logger = logger;

            TaskGuid = taskGuid;
        }

        public void Execute(
            Func<bool> unexpectedBreakFunc,
            out bool needToRepeat
            )
        {
            var checkResult = MarkSignals(
                () =>
                {
                    if (_pusher.IsWorking)
                    {
                        return
                            false;
                    }
                    if (_pusher.IsCancelled)
                    {
                        return
                            false;
                    }
                    if (_pusher.IsCompletedSuccessfully)
                    {
                        return
                            false;
                    }

                    return
                        true;
                },
                true,
                null,
                null
                );

            if (!checkResult)
            {
                needToRepeat = false;
                return;
            }
            
            var result = DoWork(
                unexpectedBreakFunc
                );

            needToRepeat = !result;

            if (result)
            {
                MarkSignals(
                    false,
                    null,
                    true
                    );
            }
            else
            {
                MarkSignals(
                    false,
                    null,
                    false
                    );
            }
        }

        public void Serialize(
            XmlNode target
            )
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            var taskGuidNode = target.OwnerDocument.CreateNode(
                XmlNodeType.Element,
                "taskguid",
                null
                );
            taskGuidNode.InnerText = TaskGuid.ToString();
            target.AppendChild(taskGuidNode);

            var pusherNode = target.OwnerDocument.CreateNode(
                XmlNodeType.Element,
                "pusher",
                null
                );
            _pusher.Serialize(pusherNode);
            target.AppendChild(pusherNode);

            var pushTimeoutAfterFailureMsecNode = target.OwnerDocument.CreateNode(
                XmlNodeType.Element,
                "pushTimeoutAfterFailureMsec",
                null
                );
            pushTimeoutAfterFailureMsecNode.InnerText = _pushTimeoutAfterFailureMsec.ToString();
            target.AppendChild(pushTimeoutAfterFailureMsecNode);
        }


        private bool DoWork(
            Func<bool> unexpectedBreakFunc
            )
        {
            if (unexpectedBreakFunc == null)
            {
                throw new ArgumentNullException("unexpectedBreakFunc");
            }

            _pusher.SetProgress(0f);

            var dlpSuccess = false;
            var drpSuccess = false;
            try
            {
                //-------------- local preparation ------------------

                dlpSuccess = SafelyDoLocalPreparation();

                if (!dlpSuccess)
                {
                    return
                        false;
                }

                //---------------- remote preparation --------------

                var drpResult = SafelyDoRemotePreparation();

                switch (drpResult)
                {
                    case RemotePreparationResultEnum.AllowToContinue:
                        drpSuccess = true;
                        break;
                    case RemotePreparationResultEnum.AlreadyCommitted:
                        return true;
                    case RemotePreparationResultEnum.ShouldRepeat:
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                //-------------- do remote work -----------------

                do
                {
                    if (unexpectedBreakFunc())
                    {
                        //leave uncompleted transaction untouched; it will be deleted by the next exclusive access

                        return
                            false;
                    }

                    var pp = SafelyDoRemoteIteration();
                    _pusher.SetProgress(pp);
                }
                while (_pusher.Progress < 1f);

                //-------------------------------------------------------
            }
            finally
            {
                if (drpSuccess)
                {
                    SafelyCloseRemoteInfrastructure();
                }

                if (dlpSuccess)
                {
                    SafelyCloseLocalInfrastructure();
                }
            }

            return
                true;
        }

        #region abstract methods

        protected abstract void DoLocalPreparation();

        protected abstract void DoRemotePreparation(
            out RemotePreparationResultEnum result
            );

        protected abstract float DoRemoteIteration(
            );

        protected abstract void SafelyCloseRemoteInfrastructure();

        protected abstract void SafelyCloseLocalInfrastructure();

        #endregion

        #region safely wrapped abstract methods

        private bool SafelyDoLocalPreparation()
        {
            var result = false;
            try
            {
                DoLocalPreparation();

                result = true;
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);
            }

            return
                result;
        }

        private RemotePreparationResultEnum SafelyDoRemotePreparation(
            )
        {
            RemotePreparationResultEnum result;
            try
            {
                DoRemotePreparation(
                    out result
                    );
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);

                result = RemotePreparationResultEnum.ShouldRepeat;
            }

            return
                result;
        }

        private float SafelyDoRemoteIteration()
        {
            var result = 0f;
            try
            {
                result = DoRemoteIteration();
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);
            }

            return
                result;
        }

        #endregion

        #region private methods

        private bool MarkSignals(
            bool? isWorking,
            bool? isCancelled,
            bool? isCompletedSuccessfully
            )
        {
            return
                MarkSignals(
                    () => true,
                    isWorking,
                    isCancelled,
                    isCompletedSuccessfully
                    );
        }

        private bool MarkSignals(
            Func<bool> checkMethod,
            bool? isWorking,
            bool? isCancelled,
            bool? isCompletedSuccessfully
            )
        {
            if (checkMethod == null)
            {
                throw new ArgumentNullException("checkMethod");
            }

            lock (_locker)
            {
                var checkResult = checkMethod();
                if (checkResult)
                {
                    if (isWorking.HasValue)
                    {
                        _pusher.SetIsWorking(isWorking.Value);
                    }
                    if (isCancelled.HasValue)
                    {
                        _pusher.SetIsCancelled(isCancelled.Value);
                    }
                    if (isCompletedSuccessfully.HasValue)
                    {
                        _pusher.SetIsCompleted(isCompletedSuccessfully.Value);
                    }
                }

                return
                    checkResult;
            }
        }

        #endregion

        #region private classes

        private class AlgorithmException : Exception
        {
            public AlgorithmException()
            {
            }

            public AlgorithmException(string message) : base(message)
            {
            }

            public AlgorithmException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected AlgorithmException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }

        #endregion
    }
}
