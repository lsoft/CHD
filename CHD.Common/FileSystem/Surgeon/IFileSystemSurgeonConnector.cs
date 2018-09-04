namespace CHD.Common.FileSystem.Surgeon
{
    public interface IFileSystemSurgeonConnector
    {
        IFileSystemConnector Connector
        {
            get;
        }

        IFileSystemSurgeon OpenSurgeon();
    }
}