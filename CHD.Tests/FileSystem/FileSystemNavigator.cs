using System;
using CHD.Common;
using CHD.Common.FileSystem;
using CHD.Common.FileSystem.FFile;
using CHD.Common.FileSystem.FFolder;
using CHD.Tests.FileSystem.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CHD.Tests.FileSystem
{
    public sealed class FileSystemNavigator
    {
        private readonly IDisorderLogger _logger;
        private readonly IFileSystem _fileSystem;
        private IFolder _currentFolder;

        public FileSystemNavigator(
            IFileSystemConnector fileSystemConnector,
            IDisorderLogger logger
            )
        {
            if (fileSystemConnector == null)
            {
                throw new ArgumentNullException("fileSystemConnector");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _logger = logger;

            var fileSystem = fileSystemConnector.Open();

            _fileSystem = fileSystem;
            _currentFolder = fileSystem.RootFolder;
        }


        public FileSystemNavigator Enter(string folderName)
        {
            var child = _currentFolder.GetFolderByName(folderName);

            if (child == null)
            {
                throw new InternalTestFailureException(folderName + " is not found in " + _currentFolder.Name);
            }

            _currentFolder = child;

            return
                this;
        }

        public FileSystemNavigator Enter(params string[] folderNames)
        {
            FileSystemNavigator s = this;
            foreach (var fn in folderNames)
            {
                s = s.Enter(fn);
            }

            return
                s;
        }

        public FileSystemNavigator Up()
        {
            if (_currentFolder.Parent == null)
            {
                throw new InvalidOperationException("parent == null");
            }

            _currentFolder =  _currentFolder.Parent;

            return
                this;
        }

        public FileSystemNavigator GetFile(string fileName, out IFile result)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            result = _currentFolder.GetFileByName(fileName);

            return
                this;
        }

        public FileSystemNavigator GetFolder(
            string folderName,
            out IFolder result
            )
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName");
            }

            result = _currentFolder.GetFolderByName(folderName);

            return
                this;
        }


        public IFileSystemSchema CloseFileSystem()
        {
            var result = StructureFileSystemSchemaFactory.CreateSchema(
                _fileSystem.PathComparerProvider,
                _fileSystem.RootFolder,
                _logger
                );

            //no need to commit, there are no changes maked
            _fileSystem.Dispose();

            return
                result;
        }
    }
}