using System;
using System.Xml;
using CHD.Common.Logger;
using CHD.FileSystem.FileWrapper;
using CHD.Push.ActivityPool;

namespace CHD.Push.Pusher.Factory
{
    public class PusherFactory : IPusherFactory
    {
        private readonly IFileWrapperFactory _fileWrapperFactory;
        private readonly IDisorderLogger _logger;

        public PusherFactory(
            IFileWrapperFactory fileWrapperFactory,
            IDisorderLogger logger
            )
        {
            if (fileWrapperFactory == null)
            {
                throw new ArgumentNullException("fileWrapperFactory");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _fileWrapperFactory = fileWrapperFactory;
            _logger = logger;
        }

        public IPusher Load(
            XmlNode source
            )
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var fileWrapperNode = source.SelectSingleNode("filewrapper");
            var fileWrapper = _fileWrapperFactory.CreateFile(fileWrapperNode);

            var typeNode = source.SelectSingleNode("type");
            var type = (ActivityTypeEnum)Enum.Parse(typeof(ActivityTypeEnum), typeNode.InnerText);

            var pusher = new Pusher(
                fileWrapper,
                type,
                _logger
                );

            return
                pusher;
        }

        public IPusher CreatePusher(
            ActivityTypeEnum activity,
            IFileWrapper fileWrapper
            )
        {
            if (fileWrapper == null)
            {
                throw new ArgumentNullException("fileWrapper");
            }

            var pusher = new Pusher(
                fileWrapper,
                activity,
                _logger
                );

            return
                pusher;
        }
    }
}