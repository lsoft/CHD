using System;
using CHD.Common.PathComparer;
using CHD.Common.Saver;
using CHD.Common.Structure.Cleaner;

namespace CHD.Common.Structure.Container.Factory
{
    public sealed class StructureContainerFactory<TAddress> : IStructureContainerFactory
        where TAddress : IAddress
    {
        private readonly IPathComparerProvider _pathComparerProvider;
        private readonly IBinarySaver<TAddress> _saver;
        private readonly TAddress _structureAddress;
        private readonly IStructureCleaner _cleaner;
        private readonly IDisorderLogger _logger;

        public StructureContainerFactory(
            IPathComparerProvider pathComparerProvider,
            IBinarySaver<TAddress> saver,
            TAddress structureAddress,
            IStructureCleaner cleaner,
            IDisorderLogger logger
            )
        {
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }
            if (saver == null)
            {
                throw new ArgumentNullException("saver");
            }
            if (structureAddress == null)
            {
                throw new ArgumentNullException("structureAddress");
            }
            if (cleaner == null)
            {
                throw new ArgumentNullException("cleaner");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _pathComparerProvider = pathComparerProvider;
            _saver = saver;
            _structureAddress = structureAddress;
            _cleaner = cleaner;
            _logger = logger;
        }

        public IStructureContainer CreateStructure(
            string rootFolderName
            )
        {
            if (rootFolderName == null)
            {
                throw new ArgumentNullException("rootFolderName");
            }

            var result = new StructureContainer<TAddress>(
                rootFolderName,
                _pathComparerProvider,
                _saver,
                _structureAddress,
                _logger
                );

            result.Cleanup(_cleaner);

            return
                result;
        }
    }
}