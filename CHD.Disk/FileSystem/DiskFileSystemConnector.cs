using System;
using CHD.Common;
using CHD.Common.FileSystem;
using CHD.Common.PathComparer;
using CHD.Common.Structure.Container;
using CHD.Common.Structure.Container.Factory;

namespace CHD.Disk.FileSystem
{
    public sealed class DiskFileSystemConnector : IFileSystemConnector
    {
        private readonly IPathComparerProvider _pathComparerProvider;
        private readonly IStructureContainerFactory _structureContainerFactory;
        private readonly string _rootFolderName;
        private readonly string _preRootPath;
        private readonly IDisorderLogger _logger;

        public string Name
        {
            get
            {
                return
                    typeof(DiskFileSystem).Name;
            }
        }


        public DiskFileSystemConnector(
            IPathComparerProvider pathComparerProvider,
            IStructureContainerFactory structureContainerFactory,
            string rootFolderName,
            string preRootPath,
            IDisorderLogger logger
            )
        {
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }
            if (structureContainerFactory == null)
            {
                throw new ArgumentNullException("structureContainerFactory");
            }
            if (rootFolderName == null)
            {
                throw new ArgumentNullException("rootFolderName");
            }
            if (preRootPath == null)
            {
                throw new ArgumentNullException("preRootPath");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _pathComparerProvider = pathComparerProvider;
            _structureContainerFactory = structureContainerFactory;
            _rootFolderName = rootFolderName;
            _preRootPath = preRootPath;
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

        private DiskFileSystem OpenTyped(
            )
        {
            var structureContainer = ReadStructureContainer();

            var result = OpenTyped(structureContainer);

            return
                result;
        }

        private DiskFileSystem OpenTyped(
            IStructureContainer structureContainer
            )
        {
            if (structureContainer == null)
            {
                throw new ArgumentNullException("structureContainer");
            }

            var r = new DiskFileSystem(
                structureContainer,
                _pathComparerProvider,
                _preRootPath,
                _logger
                );

            return
                r;
        }
    }
}