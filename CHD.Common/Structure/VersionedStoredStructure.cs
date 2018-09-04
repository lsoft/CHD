using System;
using System.Collections.Generic;
using System.Linq;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.Serializer;
using CHD.Common.Structure.Cleaner;

namespace CHD.Common.Structure
{
    [Serializable]
    public sealed class VersionedStoredStructure : IVersionedStoredStructure
    {
        public SerializationVersionProvider<VersionedStoredStructure> SerializationVersion = new SerializationVersionProvider<VersionedStoredStructure>();

        private readonly Dictionary<int, IStoredStructure> _versions;

        private IStoredStructure _penult;
        private IStoredStructure _last;

        public IStoredStructure Penult
        {
            get
            {
                return
                    _penult;
            }
        }

        public IStoredStructure Last
        {
            get
            {
                return
                    _last;
            }
        }

        public IReadOnlyDictionary<int, IStoredStructure> Versions
        {
            get
            {
                return
                    _versions;
            }
        }

        public VersionedStoredStructure(
            IStoredStructure defaultStructure
            )
        {
            if (defaultStructure == null)
            {
                throw new ArgumentNullException("defaultStructure");
            }

            _versions = new Dictionary<int, IStoredStructure>
            {
                { defaultStructure.Version, defaultStructure }
            };

            _penult = null;
            _last = defaultStructure;
        }

        public void CreateNewVersion(
            IFolder actualRoot
            )
        {
            if (actualRoot == null)
            {
                throw new ArgumentNullException("actualRoot");
            }

            var newv = _last.CreateNew(
                actualRoot
                );

            _versions.Add(newv.Version, newv);

            _last.SetReadOnlyMode();

            _penult = _last;
            _last = newv;
        }

        public void CleanupObsoletedVersions(
            IStructureCleaner cleaner
            )
        {
            if (cleaner == null)
            {
                throw new ArgumentNullException("cleaner");
            }

            var toDelete = cleaner.FilterToCleanup(_versions.Values.ToList());

            foreach (var tod in toDelete)
            {
                _versions.Remove(tod.Version);
            }
        }

        public StructureChecker GetChecker()
        {
            return
                new StructureChecker(
                    _versions.Values
                    );
        }
    }
}