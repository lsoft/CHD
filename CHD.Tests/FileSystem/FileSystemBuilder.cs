using System;
using CHD.Common;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.FileSystem.Surgeon;
using CHD.Tests.FileSystem.Schema;
using CHD.Tests.Other;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CHD.Tests.FileSystem
{
    public sealed class FileSystemBuilder
    {
        private readonly DateTime _defaultLastWriteTimeUtc;
        private readonly IFileSystemSurgeonConnector _surgeonConnector;
        private readonly IDisorderLogger _logger;
        private readonly IFileSystemSurgeon _surgeon;

        private IFolder _currentFolder;


        public FileSystemBuilder(
            DateTime defaultLastWriteTimeUtc,
            IFileSystemSurgeonConnector surgeonConnector,
            IDisorderLogger logger
            )
        {
            if (surgeonConnector == null)
            {
                throw new ArgumentNullException("surgeonConnector");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _defaultLastWriteTimeUtc = defaultLastWriteTimeUtc;
            _surgeonConnector = surgeonConnector;
            _logger = logger;

            _surgeon = _surgeonConnector.OpenSurgeon();

            _currentFolder = _surgeon.FileSystem.RootFolder;
        }


        public FileSystemBuilder Folder(string folderName)
        {
            var child = _currentFolder.GetFolderByName(folderName);

            if (child == null)
            {
                child = _surgeon.CreateFolder(
                    _currentFolder,
                    folderName
                    );
            }

            _currentFolder = child;

            return
                this;
        }

        public FileSystemBuilder Folder(params string[] folderNames)
        {
            FileSystemBuilder b = this;

            foreach (var fn in folderNames)
            {
                b = b.Folder(fn);
            }

            return
                b;
        }

        public FileSystemBuilder Enter(string folderName)
        {

            var child = _currentFolder.GetFolderByName(folderName);

            if (child == null)
            {
                throw new InternalTestFailureException(folderName + " is not found in " + _currentFolder.FullPath);
            }

            _currentFolder = child;

            return
                this;
        }

        public FileSystemBuilder Up(int cnt)
        {
            FileSystemBuilder b = this;

            for (var cc = 0; cc < cnt; cc++)
            {
                b = b.Up();
            }

            return b;
        }

        public FileSystemBuilder Up()
        {
            _currentFolder = _currentFolder.Parent;

            return
                this;
        }

        public FileSystemBuilder EmptyFile(
            string fileName
            )
        {
            var body = new byte[0];

            _surgeon.CreateFileWithBody(
                _currentFolder,
                fileName,
                body,
                _defaultLastWriteTimeUtc //ChangeIdentifierHelper.MakeForTestingPurpose(_currentFolder, fileName, body)
                );

            return
                this;
        }

        public FileSystemBuilder PseudoRandomFile(
            string fileName,
            int seed,
            long size
            )
        {
            var body = CHD.Tests.Other.TestHelper.GenerateBufferWithRandomData(seed, size);

            _surgeon.CreateFileWithBody(
                _currentFolder,
                fileName,
                body,
                _defaultLastWriteTimeUtc //ChangeIdentifierHelper.MakeForTestingPurpose(_currentFolder, fileName, body)
                );

            return
                this;
        }

        public FileSystemBuilder RandomFile(
            string fileName,
            long size
            )
        {
            var body = CHD.Tests.Other.TestHelper.GenerateBufferWithRandomData(size);

            _surgeon.CreateFileWithBody(
                _currentFolder,
                fileName,
                body,
                _defaultLastWriteTimeUtc //ChangeIdentifierHelper.MakeForTestingPurpose(_currentFolder, fileName, body)
                );

            return
                this;
        }


        public FileSystemBuilder File(
            string fileName,
            byte[] body
            )
        {
            _surgeon.CreateFileWithBody(
                _currentFolder,
                fileName,
                body,
                _defaultLastWriteTimeUtc //ChangeIdentifierHelper.MakeForTestingPurpose(_currentFolder, fileName, body)
                );

            return
                this;
        }

        public IFileSystemSurgeonConnector Create(
            out IFileSystemSchema schema
            )
        {
            _surgeon.SafelyCommit();

            schema = StructureFileSystemSchemaFactory.CreateSchema(
                _surgeon.FileSystem.PathComparerProvider,
                _surgeon.FileSystem.RootFolder,
                _logger
                );

            return
                _surgeonConnector;
        }

    }
}