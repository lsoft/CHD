using System;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.Operation;
using CHD.Common.PathComparer;
using CHD.Common.Saver;
using CHD.Common.Structure.Cleaner;

namespace CHD.Common.Structure.Container
{
    /// <summary>
    /// Ќаивный хранитель лога операций
    /// </summary>
    public sealed class StructureContainer<TAddress> : IStructureContainer
        where TAddress : IAddress
    {
        private readonly object _locker = new object();

        private readonly IPathComparerProvider _pathComparerProvider;
        private readonly IBinarySaver<TAddress> _saver;
        private readonly TAddress _structureAddress;
        private readonly IDisorderLogger _logger;

        private readonly IVersionedStoredStructure _structure;

        public IVersionedStoredStructure Structure
        {
            get
            {
                return
                    _structure;
            }
        }

        public IStoredStructure Last
        {
            get
            {
                return
                    _structure.Last;
            }
        }

        public StructureContainer(
            string rootFolderName,
            IPathComparerProvider pathComparerProvider,
            IBinarySaver<TAddress> saver,
            TAddress structureAddress,
            IDisorderLogger logger
            )
        {
            if (rootFolderName == null)
            {
                throw new ArgumentNullException("rootFolderName");
            }
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
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _pathComparerProvider = pathComparerProvider;
            _saver = saver;
            _structureAddress = structureAddress;
            _logger = logger;

            _structure = DoReadStructure(rootFolderName);
        }

        public void SaveActual(
            IFolder actualRoot
            )
        {
            if (actualRoot == null)
            {
                throw new ArgumentNullException("actualRoot");
            }

            var cloned = _saver.Serializer.DeepClone(actualRoot);

            _structure.CreateNewVersion(
                cloned
                );
        }

        /// <summary>
        /// ќчистить структуру от устаревших версий
        /// </summary>
        public void Cleanup(
            IStructureCleaner cleaner
            )
        {
            if (cleaner == null)
            {
                throw new ArgumentNullException("cleaner");
            }

            _structure.CleanupObsoletedVersions(cleaner);

            _logger.LogMessage("Structure cleanuped");
        }

        public void Save()
        {
            lock (_locker)
            {
                DoSaveStructure();
            }
        }

        public StructureChecker GetChecker(
            )
        {
            var result = this._structure.GetChecker();

            return
                result;
        }

        #region private code

        private IVersionedStoredStructure DoReadStructure(
            string rootFolderName
            )
        {
            if (!_saver.IsTargetExists(_structureAddress))
            {
                return
                    new VersionedStoredStructure(
                        new StoredStructure(
                            _pathComparerProvider,
                            rootFolderName
                            )
                        );
            }

            var result = _saver.Read<VersionedStoredStructure>(_structureAddress);

            return result;
        }

        private void DoSaveStructure(
            )
        {
            _saver.Save(
                _structureAddress,
                _structure
                );
        }

        #endregion
    }
}