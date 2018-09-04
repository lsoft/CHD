using System;
using System.IO;
using CHD.Common.FileSystem;
using CHD.Common.FileSystem.FFile;
using CHD.Common.Others;

namespace CHD.Disk.FileSystem
{
    public sealed class DiskCopier : IFileSystemCopier
    {
        private readonly string _preRootPath;

        public DiskCopier(
            string preRootPath
            )
        {
            if (preRootPath == null)
            {
                throw new ArgumentNullException("preRootPath");
            }
            _preRootPath = preRootPath;
        }

        public bool IsVersionedCopyingSupported
        {
            get
            {
                return
                    false;
            }
        }

        public long CopyFileTo(
            int structureVersion,
            ICopyableFile sourceFile,
            Stream destinationStream,
            long position,
            long size
            )
        {
            throw new CHDException(
                "Versioned copying is not supported for the disk file system",
                CHDExceptionTypeEnum.OperationIsNotSupported
                );
        }

        public long CopyFileTo(
            ICopyableFile sourceFile,
            Stream destinationStream,
            long position,
            long size
            )
        {
            if (sourceFile == null)
            {
                throw new ArgumentNullException("sourceFile");
            }
            if (destinationStream == null)
            {
                throw new ArgumentNullException("destinationStream");
            }
            if (!destinationStream.CanWrite)
            {
                throw new InvalidOperationException("destination stream cannot write data");
            }

            var absolutePath = Path.Combine(_preRootPath, sourceFile.FullPath);

            long copiedSize;
            using (var source = new FileStream(absolutePath, FileMode.Open, FileAccess.Read))
            {
                source.Position = position;

                copiedSize = source.CopyToConstraint(destinationStream, size);
            }

            File.SetLastWriteTimeUtc(
                absolutePath,
                sourceFile.LastWriteTimeUtc
                );

            //Debug.WriteLine(
            //    "¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤ {0}: {1}",
            //    absolutePath,
            //    sourceFile.LastWriteTimeUtc.Ticks
            //    );

            return
                copiedSize;
        }
    }
}