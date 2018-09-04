using CHD.Common.FileSystem.FFolder;

namespace CHD.Common.Scanner
{
    public interface IScanner
    {
        IFolder Scan();
    }
}