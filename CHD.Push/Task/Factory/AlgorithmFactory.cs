using System;
using System.Xml;
using CHD.Common.Logger;
using CHD.FileSystem.FileWrapper;
using CHD.Graveyard.Graveyard;
using CHD.Push.ActivityPool;
using CHD.Push.Pusher.Factory;

namespace CHD.Push.Task.Factory
{
    public class AlgorithmFactory : IAlgorithmFactory
    {
        private readonly IPusherFactory _pusherFactory;
        private readonly IGraveyard _graveyard;
        private readonly long _blockFileSize;
        private readonly IDisorderLogger _logger;

        public AlgorithmFactory(
            IPusherFactory pusherFactory,
            IGraveyard graveyard,
            long blockFileSize,
            IDisorderLogger logger
            )
        {
            if (pusherFactory == null)
            {
                throw new ArgumentNullException("pusherFactory");
            }
            if (graveyard == null)
            {
                throw new ArgumentNullException("graveyard");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _pusherFactory = pusherFactory;
            _graveyard = graveyard;
            _blockFileSize = blockFileSize;
            _logger = logger;
        }

        public IAlgorithm Load(
            XmlNode source
            )
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            IAlgorithm result = null;

            var pusherNode = source.SelectSingleNode("pusher");

            var pusher = _pusherFactory.Load(
                pusherNode
                );

            var taskGuid = Guid.Parse(source.SelectSingleNode("taskguid").InnerText);
            var pushTimeoutAfterFailureMsec = int.Parse(source.SelectSingleNode("pushTimeoutAfterFailureMsec").InnerText);

            switch (pusher.Type)
            {
                case ActivityTypeEnum.CreateOrChange:
                    result = new CreateOrChangeAlgorithm(
                        pusher,
                        _graveyard,
                        taskGuid,
                        _blockFileSize,
                        pushTimeoutAfterFailureMsec,
                        _logger
                        );
                    break;
                case ActivityTypeEnum.Delete:
                    result = new DeleteFileAlgorithm(
                        pusher,
                        _graveyard,
                        taskGuid,
                        pushTimeoutAfterFailureMsec,
                        _logger
                        );
                    break;
                default:
                    throw new ArgumentOutOfRangeException(pusher.Type.ToString());
            }

            return
                result;
        }

        public IAlgorithm Create(
            Guid taskGuid,
            ActivityTypeEnum type,
            IFileWrapper fileWrapper,
            int pushTimeoutAfterFailureMsec
            )
        {
            if (fileWrapper == null)
            {
                throw new ArgumentNullException("fileWrapper");
            }

            var pusher = _pusherFactory.CreatePusher(
                type,
                fileWrapper
                );

            switch (type)
            {
                case ActivityTypeEnum.CreateOrChange:
                    return
                        new CreateOrChangeAlgorithm(
                            pusher,
                            _graveyard,
                            taskGuid,
                            _blockFileSize,
                            pushTimeoutAfterFailureMsec,
                            _logger
                            );
                case ActivityTypeEnum.Delete:
                    return
                        new DeleteFileAlgorithm(
                            pusher,
                            _graveyard,
                            taskGuid,
                            pushTimeoutAfterFailureMsec,
                            _logger
                            );
                default:
                    throw new ArgumentOutOfRangeException("type = " + type);
            }
        }
    }
}
