

using CHD.Common.Structure.Container;

namespace CHD.Common.FileSystem
{
    public interface IFileSystemConnector
    {
        IStructureContainer ReadStructureContainer(
            );

        IFileSystem Open(
            IStructureContainer structureContainer
            );

        IFileSystem Open(
            );

        string Name
        {
            get;
        }
    }
}