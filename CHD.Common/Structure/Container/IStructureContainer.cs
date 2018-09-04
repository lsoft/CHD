using CHD.Common.FileSystem.FFolder;
using CHD.Common.Operation;

namespace CHD.Common.Structure.Container
{
    public interface IStructureContainer
    {
        IVersionedStoredStructure Structure
        {
            get;
        }

        IStoredStructure Last
        {
            get;
        }

        void SaveActual(
            IFolder actualRoot
            );

        void Save();

        StructureChecker GetChecker(
            );
    }
}