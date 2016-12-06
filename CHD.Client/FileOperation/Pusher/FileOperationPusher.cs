using System;
using System.Xml;
using CHD.Client.FileOperation.Container;
using CHD.Common.Logger;
using CHD.FileSystem.Algebra;
using CHD.FileSystem.FileWrapper;
using CHD.Push.ActivityPool;
using CHD.Push.Pusher;

namespace CHD.Client.FileOperation.Pusher
{
    internal class FileOperationPusher : IPusher, IFileOperation
    {
        private readonly IPusher _pusher;
        private readonly IActualFileOperationsContainer _actualFileOperationsContainer;
        private readonly IDisorderLogger _logger;

        public IFileWrapper FileWrapper
        {
            get
            {
                return
                    _pusher.FileWrapper;
            }
        }

        public ActivityTypeEnum Type
        {
            get
            {
                return
                    _pusher.Type;
            }
        }

        public Suffix FilePathSuffix
        {
            get
            {
                return
                    _pusher.FileWrapper.FilePathSuffix;
            }
        }

        public float Progress
        {
            get
            {
                return
                    _pusher.Progress;
            }
        }

        public bool IsWorking
        {
            get
            {
                return
                    _pusher.IsWorking;
            }
        }

        public bool IsCancelled
        {
            get
            {
                return
                    _pusher.IsCancelled;
            }
        }

        public bool IsCompletedSuccessfully
        {
            get
            {
                return
                    _pusher.IsCompletedSuccessfully;
            }
        }

        public bool IsDead
        {
            get
            {
                return
                    _pusher.IsDead;
            }
        }

        public event ProgressChangedDelegate ProgressChangedEvent;

        public FileOperationPusher(
            IPusher pusher,
            IActualFileOperationsContainer actualFileOperationsContainer,
            ActivityTypeEnum type,
            IDisorderLogger logger
            )
        {
            if (pusher == null)
            {
                throw new ArgumentNullException("pusher");
            }
            if (actualFileOperationsContainer == null)
            {
                throw new ArgumentNullException("actualFileOperationsContainer");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _pusher = pusher;
            _actualFileOperationsContainer = actualFileOperationsContainer;
            _logger = logger;


            FileActionTypeEnum fate;

            switch (type)
            {
                case ActivityTypeEnum.CreateOrChange:
                    fate = FileActionTypeEnum.Upload;
                    break;
                case ActivityTypeEnum.Delete:
                    fate = FileActionTypeEnum.Delete;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _actualFileOperationsContainer.AddOperation(this, fate);
        }

        public void SetProgress(float progress)
        {
            _pusher.SetProgress(progress);

            OnProgressChanged(progress);
        }

        public void SetIsWorking(bool isWorking)
        {
            _pusher.SetIsWorking(isWorking);

            if (isWorking)
            {
                _actualFileOperationsContainer.StartOperation(this);
            }
        }

        public void SetIsCancelled(bool isCancelled)
        {
            _pusher.SetIsCancelled(isCancelled);

            if (isCancelled)
            {
                _actualFileOperationsContainer.RemoveOperation(this);
            }
        }

        public void SetIsCompleted(bool isCompletedSuccessfully)
        {
            _pusher.SetIsCompleted(isCompletedSuccessfully);

            if (isCompletedSuccessfully)
            {
                _actualFileOperationsContainer.RemoveOperation(this);
            }
        }

        public void Serialize(XmlNode target)
        {
            _pusher.Serialize(target);
        }


        protected virtual void OnProgressChanged(float progress)
        {
            ProgressChangedDelegate handler = ProgressChangedEvent;
            if (handler != null)
            {
                try
                {
                    handler(progress);
                }
                catch (Exception excp)
                {
                    _logger.LogException(excp);
                }
            }
        }

    }
}
