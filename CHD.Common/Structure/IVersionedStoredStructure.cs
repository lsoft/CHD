using System.Collections.Generic;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.Serializer;
using CHD.Common.Structure.Cleaner;

namespace CHD.Common.Structure
{
    public interface IVersionedStoredStructure
    {
        IStoredStructure Penult
        {
            get;
        }
        
        IStoredStructure Last
        {
            get;
        }

        IReadOnlyDictionary<int, IStoredStructure> Versions
        {
            get;
        }

        void CreateNewVersion(
            IFolder actualRoot
            );

        void CleanupObsoletedVersions(
            IStructureCleaner cleaner
            );
        
        StructureChecker GetChecker(
            );
    }
}