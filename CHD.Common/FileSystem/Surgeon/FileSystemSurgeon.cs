using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using CHD.Common.FileSystem.FFile;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.Operation;
using CHD.Common.Operation.Fixer;
using CHD.Common.Operation.FolderOperation;
using CHD.Common.Operation.Visitor.Splitter;
using CHD.Common.Others;

namespace CHD.Common.FileSystem.Surgeon
{
    public sealed class FileSystemSurgeon : IFileSystemSurgeon
    {
        private readonly IFileSystem _targetFileSystem;
        private readonly IDisorderLogger _logger;

        public IFileSystem FileSystem
        {
            get
            {
                return
                    _targetFileSystem;
            }
        }

        private long _cleanuped;

        public FileSystemSurgeon(
            IFileSystem targetFileSystem,
            IDisorderLogger logger
            )
        {
            if (targetFileSystem == null)
            {
                throw new ArgumentNullException("targetFileSystem");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _targetFileSystem = targetFileSystem;
            _logger = logger;
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _cleanuped, 1L) == 0L)
            {
                _targetFileSystem.Dispose();
            }
        }

        public void SafelyCommit()
        {
            if (Interlocked.Exchange(ref _cleanuped, 1L) == 0L)
            {
                _targetFileSystem.SafelyCommit();
            }
        }



        public IFolder CreateFolder(
            IFolder folder,
            string folderName
            )
        {
            if (folder == null)
            {
                throw new ArgumentNullException("folder");
            }
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName");
            }

            var changeIdentifier = ChangeIdentifierHelper.MakeForFolder(
                folder,
                folderName
                );

            var fakeFolder = new FakeNamedFolder(
                folderName,
                Path.Combine(folder.FullPath, folderName),
                new PathSequence(folder.FullPathSequence, folderName),
                changeIdentifier
                );

            var fixer = _targetFileSystem.Executor.CreateChildFolder(
                fakeFolder,
                folder
                );

            fixer.SafelyCommit();

            return
                fixer.Result;
        }

        public IFile CreateFileWithBody(
            IFolder folder,
            string fileName,
            byte[] fileBody,
            DateTime lastWriteTimeUtc = default(DateTime)
            )
        {
            if (folder == null)
            {
                throw new ArgumentNullException("folder");
            }
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            if (fileBody == null)
            {
                throw new ArgumentNullException("fileBody");
            }

            if (lastWriteTimeUtc == default(DateTime))
            {
                lastWriteTimeUtc = DateTime.UtcNow;
            }

            var changeIdentifier = ChangeIdentifierHelper.MakeForFile(
                folder,
                fileName,
                lastWriteTimeUtc
                );

            var fakeFile = new FakeCopyableFile(
                fileName,
                Path.Combine(folder.FullPath, fileName),
                fileBody,
                lastWriteTimeUtc,
                changeIdentifier
                );

            var fakeFileSystemCopier = new FakeFileSystemCopier(
                
                );

            var fixer = _targetFileSystem.Executor.CreateOrUpdateFile(
                fakeFileSystemCopier,
                fakeFile,
                folder
                );

            fixer.SafelyCommit();

            return
                fixer.Result;

        }

        public IFile ReplaceFileBody(
            IFolder folder,
            string fileName,
            Func<long, byte[]> bodyProvider,
            DateTime lastWriteTimeUtc = default(DateTime)
            )
        {
            if (folder == null)
            {
                throw new ArgumentNullException("folder");
            }
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            if (bodyProvider == null)
            {
                throw new ArgumentNullException("bodyProvider");
            }

            if (lastWriteTimeUtc == default(DateTime))
            {
                lastWriteTimeUtc = DateTime.UtcNow;
            }

            var changeIdentifier = ChangeIdentifierHelper.MakeForFile(
                folder,
                fileName,
                lastWriteTimeUtc
                );

            var f = folder.GetFileByName(fileName);

            var body = bodyProvider(f.Size);

            var fakeFile = new FakeCopyableFile(
                fileName,
                Path.Combine(folder.FullPath, fileName),
                body,
                lastWriteTimeUtc,
                changeIdentifier
                );

            var fakeFileSystemExecutor = new FakeFileSystemCopier(
                
                );

            var fixer = _targetFileSystem.Executor.CreateOrUpdateFile(
                fakeFileSystemExecutor,
                fakeFile,
                folder
                );

            fixer.SafelyCommit();

            return
                fixer.Result;
        }

        public IFile DeleteFile(
            IFolder folder,
            string fileName
            )
        {
            if (folder == null)
            {
                throw new ArgumentNullException("folder");
            }
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            IFile result = null;

            try
            {
                var fakeFile = new FakeNamedFile(
                    fileName,
                    Path.Combine(folder.FullPath, fileName)
                    );

                var fixer = _targetFileSystem.Executor.DeleteFile(
                    fakeFile,
                    folder
                    );

                fixer.SafelyCommit();

                result = fixer.Result;
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);
            }

            return result;
        }

        public IFolder DeleteFolder(
            IFolder folder,
            string folderName
            )
        {
            if (folder == null)
            {
                throw new ArgumentNullException("folder");
            }
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName");
            }

            IFolder result = null;

            try
            {

                var changeIdentifier = ChangeIdentifierHelper.MakeForFolder(
                    folder,
                    folderName
                    );

                var fakeFolder = new FakeNamedFolder(
                    folderName,
                    Path.Combine(folder.FullPath, folderName),
                    new PathSequence(folder.FullPathSequence, folderName),
                    changeIdentifier
                    );

                var fixer = _targetFileSystem.Executor.DeleteChildFolder(
                    fakeFolder,
                    folder
                    );

                fixer.SafelyCommit();

                result = fixer.Result;
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);
            }

            return result;
        }

        #region private classes

        private sealed class FakeFileSystemCopier : IFileSystemCopier
        {
            public bool IsVersionedCopyingSupported
            {
                get
                {
                    return
                        false;
                }
            }

            public FakeFileSystemCopier(
                
                )
            {
            }

            public long CopyFileTo(
                int structureVersion,
                ICopyableFile sourceFile,
                Stream destinationStream,
                long position,
                long size
                )
            {
                throw new InvalidOperationException("This operation makes no sense for FakeFileSystemCopier");
            }

            public long CopyFileTo(
                ICopyableFile sourceFile, 
                Stream destinationStream, 
                long position, 
                long size
                )
            {
                var fsource = sourceFile as FakeCopyableFile;

                if (fsource == null)
                {
                    throw new InvalidOperationException("fsource == null");
                }

                var result = fsource.CopyTo(
                    destinationStream,
                    position,
                    size
                    );

                return
                    result;
            }
        }

        private sealed class FakeNamedFolder : INamedFolder
        {
            public string Name
            {
                get;
                private set;
            }

            public string FullPath
            {
                get;
                private set;
            }

            public PathSequence FullPathSequence
            {
                get;
                private set;
            }

            public Guid ChangeIdentifier
            {
                get;
                private set;
            }


            public FakeNamedFolder(
                string name,
                string fullPath,
                PathSequence fullPathSequence,
                Guid changeIdentifier
                )
            {
                if (name == null)
                {
                    throw new ArgumentNullException("name");
                }
                if (fullPath == null)
                {
                    throw new ArgumentNullException("fullPath");
                }
                if (fullPathSequence == null)
                {
                    throw new ArgumentNullException("fullPathSequence");
                }

                Name = name;
                FullPath = fullPath;
                FullPathSequence = fullPathSequence;
                ChangeIdentifier = changeIdentifier;
            }
        }


        private class FakeNamedFile : INamedFile
        {
            public string Name
            {
                get;
                private set;
            }

            public string FullPath
            {
                get;
                private set;
            }


            public FakeNamedFile(
                string name,
                string fullPath)
            {
                if (name == null)
                {
                    throw new ArgumentNullException("name");
                }
                if (fullPath == null)
                {
                    throw new ArgumentNullException("fullPath");
                }

                Name = name;
                FullPath = fullPath;
            }
        }

        private sealed class FakeCopyableFile : FakeNamedFile, ICopyableFile
        {
            private readonly byte[] _body;

            public long Size
            {
                get
                {
                    return
                        _body.Length;
                }
            }

            public DateTime LastWriteTimeUtc
            {
                get;
                private set;
            }

            public Guid ChangeIdentifier
            {
                get;
                private set;
            }

            public FakeCopyableFile(
                string name,
                string fullPath,
                byte[] body,
                DateTime lastWriteTimeUtc,
                Guid changeIdentifier
                )
                : base(name, fullPath)
            {
                if (body == null)
                {
                    throw new ArgumentNullException("body");
                }

                _body = body;

                LastWriteTimeUtc = lastWriteTimeUtc;
                ChangeIdentifier = changeIdentifier;
            }

            public long CopyTo(Stream destinationStream, long position, long size)
            {
                if (destinationStream == null)
                {
                    throw new ArgumentNullException("destinationStream");
                }
                if (!destinationStream.CanWrite)
                {
                    throw new ArgumentException("!destinationStream.CanWrite");
                }

                destinationStream.Write(_body, (int)position, (int)size);

                return size;
            }
        }

        #endregion

    }
}