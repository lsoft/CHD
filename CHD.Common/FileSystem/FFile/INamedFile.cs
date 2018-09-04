namespace CHD.Common.FileSystem.FFile
{
    public interface INamedFile
    {
        string Name
        {
            get;
        }

        string FullPath
        {
            get;
        }


    }
}