using System;
using System.Xml;
using CHD.Common.Logger;
using CHD.FileSystem.FileWrapper;
using CHD.Push.ActivityPool;

namespace CHD.Push.Pusher
{
    public class Pusher : IPusher
    {
        private readonly IDisorderLogger _logger;


        public IFileWrapper FileWrapper
        {
            get;
            private set;
        }

        public ActivityTypeEnum Type
        {
            get;
            private set;
        }

        public float Progress
        {
            get;
            private set;
        }

        public bool IsWorking
        {
            get;
            private set;
        }

        public bool IsCancelled
        {
            get;
            private set;
        }

        public bool IsCompletedSuccessfully
        {
            get;
            private set;
        }

        public bool IsDead
        {
            get
            {
                return
                    IsCancelled || IsCompletedSuccessfully;
            }
        }

        public Pusher(
            IFileWrapper fileWrapper,
            ActivityTypeEnum type,
            IDisorderLogger logger
            )
        {
            if (fileWrapper == null)
            {
                throw new ArgumentNullException("fileWrapper");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            FileWrapper = fileWrapper;
            Type = type;

            _logger = logger;
        }

        public void SetProgress(float progress)
        {
            Progress = progress;
        }

        public void SetIsWorking(bool isWorking)
        {
            IsWorking = isWorking;
        }

        public void SetIsCancelled(bool isCancelled)
        {
            IsCancelled = isCancelled;
        }

        public void SetIsCompleted(bool isCompletedSuccessfully)
        {
            IsCompletedSuccessfully = isCompletedSuccessfully;
        }

        public void Serialize(
            XmlNode target
            )
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            var fileWrapperNode = target.OwnerDocument.CreateNode(
                XmlNodeType.Element,
                "filewrapper",
                null
                );
            FileWrapper.Serialize(fileWrapperNode);
            target.AppendChild(fileWrapperNode);

            var typeNode = target.OwnerDocument.CreateNode(
                XmlNodeType.Element,
                "type",
                null
                );
            typeNode.InnerText = Type.ToString();
            target.AppendChild(typeNode);

        }
    }
}