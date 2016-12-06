using System;
using System.Xml;
using CHD.Client.FileOperation.Container;
using CHD.Common.Logger;
using CHD.FileSystem.FileWrapper;
using CHD.Push.ActivityPool;
using CHD.Push.Pusher;
using CHD.Push.Pusher.Factory;

namespace CHD.Client.FileOperation.Pusher
{
    internal class FileOperationPusherFactory : IPusherFactory
    {
        private readonly IPusherFactory _pusherFactory;
        private readonly IActualFileOperationsContainer _actualFileOperationsContainer;
        private readonly IDisorderLogger _logger;

        public FileOperationPusherFactory(
            IPusherFactory pusherFactory,
            IActualFileOperationsContainer actualFileOperationsContainer,
            IDisorderLogger logger
            )
        {
            if (pusherFactory == null)
            {
                throw new ArgumentNullException("pusherFactory");
            }
            if (actualFileOperationsContainer == null)
            {
                throw new ArgumentNullException("actualFileOperationsContainer");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _pusherFactory = pusherFactory;
            _actualFileOperationsContainer = actualFileOperationsContainer;
            _logger = logger;
        }

        public IPusher Load(XmlNode source)
        {
            var ip = _pusherFactory.Load(
                source
                );

            var result = new FileOperationPusher(
                ip,
                _actualFileOperationsContainer,
                ip.Type,
                _logger
                );

            return
                result;
        }

        public IPusher CreatePusher(ActivityTypeEnum activity, IFileWrapper fileWrapper)
        {
            var ip = _pusherFactory.CreatePusher(activity, fileWrapper);

            var result = new FileOperationPusher(
                ip,
                _actualFileOperationsContainer,
                activity,
                _logger
                );

            return
                result;
        }
    }
}