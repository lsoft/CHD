using System;
using CHD.Common;
using CHD.Common.FileSystem;
using CHD.Common.PathComparer;
using CHD.Common.Saver.Body;
using CHD.Common.Structure.Container;
using CHD.Common.Structure.Container.Factory;
using CHD.Token;

namespace CHD.Remote.FileSystem
{
    public sealed class RemoteFileSystemConnector : IFileSystemConnector
    {
        private readonly IPathComparerProvider _pathComparerProvider;
        private readonly IStructureContainerFactory _structureContainerFactory;
        private readonly ITokenFactory _tokenFactory;
        private readonly string _rootFolderName;
        private readonly IBodyProcessor _bodyProcessor;
        private readonly IDisorderLogger _logger;

        public string Name
        {
            get;
            private set;
        }

        public RemoteFileSystemConnector(
            string name,
            IPathComparerProvider pathComparerProvider,
            IStructureContainerFactory structureContainerFactory,
            ITokenFactory tokenFactory,
            string rootFolderName,
            IBodyProcessor bodyProcessor,
            IDisorderLogger logger
            )
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }
            if (structureContainerFactory == null)
            {
                throw new ArgumentNullException("structureContainerFactory");
            }
            if (tokenFactory == null)
            {
                throw new ArgumentNullException("tokenFactory");
            }
            if (rootFolderName == null)
            {
                throw new ArgumentNullException("rootFolderName");
            }
            if (bodyProcessor == null)
            {
                throw new ArgumentNullException("bodyProcessor");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            Name = name;
            _pathComparerProvider = pathComparerProvider;
            _structureContainerFactory = structureContainerFactory;
            _tokenFactory = tokenFactory;
            _rootFolderName = rootFolderName;
            _bodyProcessor = bodyProcessor;
            _logger = logger;
        }

        public IStructureContainer ReadStructureContainer()
        {
            var result = _structureContainerFactory.CreateStructure(
                _rootFolderName
                );

            return
                result;
        }

        public IFileSystem Open(
            IStructureContainer structureContainer
            )
        {
            if (structureContainer == null)
            {
                throw new ArgumentNullException("structureContainer");
            }

            var result = OpenTyped(
                structureContainer
                );

            return
                result;
        }

        public IFileSystem Open()
        {
            var result = OpenTyped();

            return
                result;
        }

        private RemoteFileSystem OpenTyped(
            )
        {
            var structureContainer = ReadStructureContainer();

            var result = OpenTyped(structureContainer);

            return
                result;
        }

        private RemoteFileSystem OpenTyped(
            IStructureContainer structureContainer
            )
        {
            if (structureContainer == null)
            {
                throw new ArgumentNullException("structureContainer");
            }

            IToken token;
            if (!_tokenFactory.TryToObtainToken(out token))
            {
                throw new InvalidOperationException("TryToObtainToken");
            }

            RemoteFileSystem r;
            try
            {
                //токен захвачен, у нас есть эксклюзивный доступ до удаленной от нас файловой системы!
                //теперь можно читать структуру из него и т.п.

                r = new RemoteFileSystem(
                    structureContainer,
                    _pathComparerProvider,
                    token,
                    _bodyProcessor,
                    _logger
                    );
            }
            catch
            {
                token.Dispose();
                throw;
            }

            return
                r;
        }
    }
}