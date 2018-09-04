using System;

namespace CHD.Common.FileSystem.Surgeon
{
    public sealed class FileSystemSurgeonConnector : IFileSystemSurgeonConnector
    {
        private readonly IFileSystemConnector _connector;
        private readonly IDisorderLogger _logger;

        public IFileSystemConnector Connector
        {
            get
            {
                return
                    _connector;
            }
        }

        public FileSystemSurgeonConnector(
            IFileSystemConnector connector,
            IDisorderLogger logger
            )
        {
            if (connector == null)
            {
                throw new ArgumentNullException("connector");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
                    
            _connector = connector;
            _logger = logger;
        }


        public IFileSystemSurgeon OpenSurgeon()
        {
            var fileSystem = _connector.Open(
                );

            return
                new FileSystemSurgeon(
                    fileSystem,
                    _logger
                    );
        }
    }
}