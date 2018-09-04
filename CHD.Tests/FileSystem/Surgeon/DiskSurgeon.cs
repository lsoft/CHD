using System;
using CHD.Common;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.FileSystem.Surgeon;
using CHD.Common.PathComparer;
using CHD.Tests.FileSystem.Schema;
using CHD.Tests.Other;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CHD.Tests.FileSystem.Surgeon
{
    internal sealed class DiskSurgeon : ISurgeon
    {
        private readonly string _rootFolderPath;
        private readonly IPathComparerProvider _pathComparerProvider;
        private readonly IDisorderLogger _logger;

        private string _currentFolder;

        public DiskSurgeon(
            string rootFolderPath,
            IPathComparerProvider pathComparerProvider,
            IDisorderLogger logger
            )
        {
            if (rootFolderPath == null)
            {
                throw new ArgumentNullException("rootFolderPath");
            }
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _rootFolderPath = rootFolderPath;
            _pathComparerProvider = pathComparerProvider;
            _currentFolder = rootFolderPath;
            _logger = logger;
        }

        public ISurgeon Enter(string folderName)
        {
            var child = _currentFolder.Enter(folderName);

            if (child == null)
            {
                throw new InternalTestFailureException(folderName + " is not found in " + _currentFolder.FolderInfo().Name);
            }

            _currentFolder = child;

            return
                this;
        }

        public ISurgeon Enter(params string[] folderNames)
        {
            ISurgeon s = this;
            foreach (var fn in folderNames)
            {
                s = s.Enter(fn);
            }

            return
                s;
        }

        public ISurgeon Up()
        {
            if (_currentFolder.Parent(_rootFolderPath, _pathComparerProvider))
            {
                throw new InvalidOperationException("parent == null");
            }

            _currentFolder = _currentFolder.Parent();

            return
                this;
        }

        public ISurgeon CreateFolder(string folderName)
        {
            var newFolder = _currentFolder.CreateFolder(
                folderName
                );

            _currentFolder = newFolder;

            return
                this;
        }

        public ISurgeon CreatePseudoRandomFile(
            string fileName,
            byte[] body
            )
        {
            _currentFolder.CreateFileWithBody(
                fileName,
                body
                );

            return
                this;
        }

        public ISurgeon ChangeFile(
            string fileName,
            int fileSeed,
            out byte[] newFileBody
            )
        {
            byte[] generatedFileBody2 = null;

            Func<long, byte[]> bodyProvider = size =>
            {
                generatedFileBody2 = TestHelper.GenerateBufferWithRandomData(fileSeed, size);

                return
                    generatedFileBody2;
            };

            var file = _currentFolder.ReplaceFileBody(
                fileName,
                bodyProvider
                );

            if (file == null)
            {
                throw new InternalTestFailureException(string.Format("{0}: does not exists in {1}", fileName, _currentFolder));
            }

            newFileBody = generatedFileBody2;

            return
                this;
        }

        public ISurgeon DeleteFile(string fileName, bool fileMustExists)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            var file = _currentFolder.DeleteFile(
                fileName
                );

            if (file == null)
            {
                if (fileMustExists)
                {
                    throw new InternalTestFailureException(string.Format("Deleting '{0}' failed in {1}", fileName, _currentFolder));
                }
            }

            return
                this;
        }

        public ISurgeon DeleteFolder(
            string folderName,
            bool folderMustExists
            )
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName");
            }

            var folder = _currentFolder.DeleteFolder(
                folderName
                );

            if (folder == null)
            {
                if (folderMustExists)
                {
                    throw new InternalTestFailureException(string.Format("Deleting '{0}' failed in {1}", folderName, _currentFolder));
                }
            }

            return
                this;
        }

        public IFileSystemSchema CloseFileSystem()
        {
            var result = DiskFileSystemSchemaFactory.CreateSchema(
                _pathComparerProvider,
                _rootFolderPath,
                _logger
                );

            return result;
        }
    }
}