namespace CHD.Common.Structure.Container.Factory
{
    public interface IStructureContainerFactory
    {
        IStructureContainer CreateStructure(
            string rootFolderName
            );
    }
}