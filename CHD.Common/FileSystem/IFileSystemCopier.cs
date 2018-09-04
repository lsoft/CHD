using System.IO;
using CHD.Common.FileSystem.FFile;

namespace CHD.Common.FileSystem
{
    public interface IFileSystemCopier
    {
        bool IsVersionedCopyingSupported
        {
            get;
        }

        long CopyFileTo(
            int structureVersion,
            ICopyableFile sourceFile,
            Stream destinationStream,
            long position,
            long size
            );

        long CopyFileTo(
            ICopyableFile sourceFile,
            Stream destinationStream,
            long position,
            long size
            );
    }
}