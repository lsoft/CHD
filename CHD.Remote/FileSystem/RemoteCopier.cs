using System;
using System.IO;
using CHD.Common.FileSystem;
using CHD.Common.FileSystem.FFile;
using CHD.Common.Saver.Body;

namespace CHD.Remote.FileSystem
{
    public sealed class RemoteCopier : IFileSystemCopier
    {
        private readonly IBodyProcessor _bodyProcessor;

        public bool IsVersionedCopyingSupported
        {
            get
            {
                return
                    true;
            }
        }

        public RemoteCopier(
            IBodyProcessor bodyProcessor
            )
        {
            if (bodyProcessor == null)
            {
                throw new ArgumentNullException("bodyProcessor");
            }

            _bodyProcessor = bodyProcessor;
        }

        public long CopyFileTo(
            int structureVersion,
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

            var result = _bodyProcessor.CopySnapshotTo(
                structureVersion,
                sourceFile,
                destinationStream,
                position,
                size
                );

            return
                result;

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

            var result = _bodyProcessor.CopyLastSnapshotTo(
                sourceFile,
                destinationStream,
                position,
                size
                );

            return
                result;

        }

    }
}