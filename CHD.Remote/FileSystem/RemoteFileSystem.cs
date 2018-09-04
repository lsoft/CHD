using System;
using System.Collections.Generic;
using System.Threading;
using CHD.Common;
using CHD.Common.FileSystem;

using CHD.Common.FileSystem.FFile;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.FileSystem.Surgeon;
using CHD.Common.Operation.FileOperation;
using CHD.Common.Operation.Fixer;
using CHD.Common.Operation.FolderOperation;
using CHD.Common.Others;
using CHD.Common.PathComparer;
using CHD.Common.Saver.Body;
using CHD.Common.Structure;
using CHD.Common.Structure.Container;
using CHD.Token;

namespace CHD.Remote.FileSystem
{
    public sealed class RemoteFileSystem : IFileSystem
    {
        private const long ActiveStatus = 0L;
        private const long CleanupStatus = 1L;


        private readonly IPathComparerProvider _pathComparerProvider;
        private readonly IToken _token;
        private readonly IStructureContainer _structure;
        private readonly IDisorderLogger _logger;

        private long _cleanuped = ActiveStatus;

        public IPathComparerProvider PathComparerProvider
        {
            get
            {
                return
                    _pathComparerProvider;
            }
        }

        public IStructureContainer StructureContainer
        {
            get
            {
                return
                    _structure;
            }
        }

        public IStoredStructure LastStructure
        {
            get
            {
                return
                    _structure.Last;
            }
        }

        public IFolder RootFolder
        {
            get
            {
                return
                    _structure.Last.RootFolder;
            }
        }

        public IFileSystemCopier Copier
        {
            get;
            private set;
        }

        public IFileSystemExecutor Executor
        {
            get;
            private set;
        }

        public RemoteFileSystem(
            IStructureContainer structureContainer,
            IPathComparerProvider pathComparerProvider,
            IToken token,
            IBodyProcessor bodyProcessor,
            IDisorderLogger logger
            )
        {
            if (structureContainer == null)
            {
                throw new ArgumentNullException("structureContainer");
            }
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }
            if (bodyProcessor == null)
            {
                throw new ArgumentNullException("bodyProcessor");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _pathComparerProvider = pathComparerProvider;
            _token = token;
            _structure = structureContainer;
            _logger = logger;

            Copier = new RemoteCopier(bodyProcessor);
            Executor = new RemoteExecutor(this, bodyProcessor, logger);

            bodyProcessor.Cleanup(
                _structure.GetChecker()
                );
        }

        public void SaveActualStructure(IFolder actualRoot)
        {
            if (actualRoot == null)
            {
                throw new ArgumentNullException("actualRoot");
            }

            _structure.SaveActual(actualRoot);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _cleanuped, CleanupStatus) == ActiveStatus)
            {
                DoRevert();
            }
        }

        public void SafelyCommit()
        {
            if (Interlocked.Exchange(ref _cleanuped, CleanupStatus) == ActiveStatus)
            {
                DoCommit();
            }
        }

        #region private methods

        private void DoCommit()
        {
            try
            {
                _structure.Save();
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);
            }

            try
            {
                _token.Dispose();
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);
            }
        }

        private void DoRevert()
        {
            try
            {
                _token.Dispose();
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);
            }
        }

        #endregion

        #region executor

        private sealed class RemoteExecutor : IFileSystemExecutor
        {
            private readonly RemoteFileSystem _fileSystem;
            private readonly IBodyProcessor _bodyProcessor;
            private readonly IDisorderLogger _logger;

            public RemoteExecutor(
                RemoteFileSystem fileSystem,
                IBodyProcessor bodyProcessor,
                IDisorderLogger logger
                )
            {
                if (fileSystem == null)
                {
                    throw new ArgumentNullException("fileSystem");
                }
                if (bodyProcessor == null)
                {
                    throw new ArgumentNullException("bodyProcessor");
                }
                if (logger == null)
                {
                    throw new ArgumentNullException("logger");
                }
                _fileSystem = fileSystem;
                _bodyProcessor = bodyProcessor;
                _logger = logger;
            }

            public IFixer<IFolder> CreateChildFolder(
                INamedFolder sourceFileSystemFolder,
                IFolder targetFileSystemParentFolder
                )
            {
                if (sourceFileSystemFolder == null)
                {
                    throw new ArgumentNullException("sourceFileSystemFolder");
                }
                if (targetFileSystemParentFolder == null)
                {
                    throw new ArgumentNullException("targetFileSystemParentFolder");
                }


                var folderExists = targetFileSystemParentFolder.GetFolderByName(sourceFileSystemFolder.Name);

                if (folderExists != null)
                {
                    throw new CHDException(
                        string.Format(
                            "unknown problem with file {0}",
                            sourceFileSystemFolder.FullPath
                            ),
                        CHDExceptionTypeEnum.FolderAlreadyExists
                        );
                }

                //создаем новую папку
                var newFolder = new StructureFolder(
                    this._fileSystem.PathComparerProvider,
                    sourceFileSystemFolder.FullPath,
                    sourceFileSystemFolder.Name,
                    sourceFileSystemFolder.ChangeIdentifier,
                    new List<IFile>(),
                    new List<IFolder>(),
                    targetFileSystemParentFolder
                    );

                targetFileSystemParentFolder.AddChildFolder(
                    newFolder
                    );

                var result = new ActionFixer<IFolder>(
                    newFolder,
                    () =>
                    {
                        //fix this operation
                        //nothing to do here
                    },
                    () =>
                    {
                        //revert
                        //nothing to do here
                    }
                    );

                return
                    result;
            }

            public IFixer<IFolder> DeleteChildFolder(
                INamedFolder sourceFileSystemFolder,
                IFolder targetFileSystemParentFolder
                )
            {
                if (sourceFileSystemFolder == null)
                {
                    throw new ArgumentNullException("sourceFileSystemFolder");
                }
                if (targetFileSystemParentFolder == null)
                {
                    throw new ArgumentNullException("targetFileSystemParentFolder");
                }


                var localFolder = targetFileSystemParentFolder.GetFolderByName(sourceFileSystemFolder.Name);

                if (localFolder == null)
                {
                    throw new CHDException(
                        sourceFileSystemFolder.FullPath,
                        CHDExceptionTypeEnum.FolderDoesNotExists
                        );
                }

                var removedFolder = targetFileSystemParentFolder.RemoveChild(localFolder);

                var result = new ActionFixer<IFolder>(
                    removedFolder,
                    () =>
                    {
                        //fix this operation
                        //nothing to do here
                    },
                    () =>
                    {
                        //revert
                        //nothing to do here
                    }
                    );

                return
                    result;
            }


            public IFixer<IFile> CreateOrUpdateFile(
                IFileSystemCopier sourceFileSystemCopier,
                ICopyableFile sourceFileSystemFile,
                IFolder targetFileSystemParentFolder
                )
            {
                if (sourceFileSystemFile == null)
                {
                    throw new ArgumentNullException("sourceFileSystemFile");
                }
                if (targetFileSystemParentFolder == null)
                {
                    throw new ArgumentNullException("targetFileSystemParentFolder");
                }

                //создаем новый файл
                var newFile = new StructureFile(
                    _fileSystem.PathComparerProvider,
                    targetFileSystemParentFolder,
                    sourceFileSystemFile.Name,
                    sourceFileSystemFile.Size,
                    sourceFileSystemFile.LastWriteTimeUtc,
                    sourceFileSystemFile.ChangeIdentifier
                    );
                
                _bodyProcessor.SaveNewSnapshot(
                    _fileSystem._structure.Last.Version,
                    sourceFileSystemCopier,
                    sourceFileSystemFile,
                    newFile
                    );

                targetFileSystemParentFolder.CreateOrUpdateChildFile(
                    newFile
                    );

                var result = new ActionFixer<IFile>(
                    newFile,
                    () =>
                    {
                        //fix this operation
                        //nothing to do here
                    },
                    () =>
                    {
                        //move file back

                        //удаляем новый файл
                        try
                        {
                            _bodyProcessor.DeleteLastSnapshot(
                                newFile
                                );
                        }
                        catch(Exception excp)
                        {
                            _logger.LogException(excp);
                        }
                    }
                    );

                return
                    result;
            }

            public IFixer<IFile> DeleteFile(
                INamedFile sourceFileSystemFile,
                IFolder targetFileSystemParentFolder
                )
            {
                if (sourceFileSystemFile == null)
                {
                    throw new ArgumentNullException("sourceFileSystemFile");
                }
                if (targetFileSystemParentFolder == null)
                {
                    throw new ArgumentNullException("targetFileSystemParentFolder");
                }

                var localFile = targetFileSystemParentFolder.GetFileByName(sourceFileSystemFile.Name);

                if (localFile == null)
                {
                    throw new CHDException(
                        sourceFileSystemFile.FullPath,
                        CHDExceptionTypeEnum.FileNotFound
                        );
                }

                var deletedFile = targetFileSystemParentFolder.RemoveChild(localFile);

                var result = new ActionFixer<IFile>(
                    deletedFile,
                    () =>
                    {
                        //fix this operation
                        //nothing to do here
                    },
                    () =>
                    {
                        //revert
                        //nothing to do here
                    }
                    );

                return
                    result;
            }


        }

        #endregion
    }
}
