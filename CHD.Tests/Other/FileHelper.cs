using System;
using System.IO;
using CHD.Common.FileSystem;
using CHD.Common.FileSystem.FFile;
using CHD.Common.FileSystem.Surgeon;
using CHD.Common.Others;
using CHD.Common.Structure;
using CHD.Common.Structure.Container;

namespace CHD.Tests.Other
{
    internal static class FileHelper
    {
        public static byte[] ExtractBody(
            this IFile file,
            Func<IStructureContainer, int> structureVersionProvider,
            IFileSystemConnector fileSystemConnector
            )
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            if (fileSystemConnector == null)
            {
                throw new ArgumentNullException("fileSystemConnector");
            }

            using (var fileSystem = fileSystemConnector.Open())
            {
                var structureVersion = structureVersionProvider(fileSystem.StructureContainer);

                var b = ReadBody(
                    file,
                    structureVersion,
                    fileSystem
                    );

                var r = b.CloneArray();

                return r;
            }
        }

        public static byte[] ExtractBody(
            this IFile file,
            IFileSystemConnector fileSystemConnector
            )
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            if (fileSystemConnector == null)
            {
                throw new ArgumentNullException("fileSystemConnector");
            }

            using(var fileSystem = fileSystemConnector.Open())
            {
                var b = ReadBody(
                    file,
                    fileSystem
                    );

                var r = b.CloneArray();

                return r;
            }
        }


        public static byte[] ReadBody(
            this IFile file,
            int structureVersion,
            IFileSystem fileSystem
            )
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            if (fileSystem == null)
            {
                throw new ArgumentNullException("fileSystem");
            }

            using (var ms = new MemoryStream((int)file.Size))
            {
                fileSystem.Copier.CopyFileTo(
                    structureVersion,
                    file,
                    ms,
                    0,
                    file.Size
                    );

                return
                    ms.ToArray();
            }
        }

        public static byte[] ReadBody(
            this IFile file,
            IFileSystem fileSystem
            )
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            if (fileSystem == null)
            {
                throw new ArgumentNullException("fileSystem");
            }

            using (var ms = new MemoryStream((int)file.Size))
            {
                fileSystem.Copier.CopyFileTo(
                    file,
                    ms,
                    0,
                    file.Size
                    );

                return
                    ms.ToArray();
            }
        }
    }
}