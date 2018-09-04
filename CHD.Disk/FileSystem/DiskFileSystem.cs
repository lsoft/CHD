using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using CHD.Common;
using CHD.Common.FileSystem;
using CHD.Common.FileSystem.FFile;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.Operation.Fixer;
using CHD.Common.Others;
using CHD.Common.PathComparer;
using CHD.Common.Structure;
using CHD.Common.Structure.Container;

namespace CHD.Disk.FileSystem
{
    public sealed class DiskFileSystem : IFileSystem
    {
        private const long ActiveStatus = 0L;
        private const long CleanupStatus = 1L;

        private readonly string _preRootPath;
        private readonly IDisorderLogger _logger;
        private readonly IPathComparerProvider _pathComparerProvider;
        private readonly IStructureContainer _structure;

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

        public DiskFileSystem(
            IStructureContainer structureContainer,
            IPathComparerProvider pathComparerProvider,
            string preRootPath,
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
            if (preRootPath == null)
            {
                throw new ArgumentNullException("preRootPath");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            preRootPath = Path.GetFullPath(preRootPath);

            _pathComparerProvider = pathComparerProvider;
            _preRootPath = preRootPath;
            _logger = logger;

            Copier = new DiskCopier(this._preRootPath);
            Executor = new DiskExecutor(this);
            _structure = structureContainer;
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
                //nothing to do
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
        }

        #endregion

        #region executor

        private sealed class DiskExecutor : IFileSystemExecutor
        {
            private readonly DiskFileSystem _fileSystem;

            public DiskExecutor(
                DiskFileSystem fileSystem
                )
            {
                if (fileSystem == null)
                {
                    throw new ArgumentNullException("fileSystem");
                }
                _fileSystem = fileSystem;
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

                //где надо создать папку
                var targetAbsolutePath = Path.Combine(
                    _fileSystem._preRootPath,
                    sourceFileSystemFolder.FullPath
                    );

                var folderExists = Directory.Exists(targetAbsolutePath);

                if (folderExists)
                {
                    throw new CHDException(
                        string.Format(
                            "unknown problem with folder {0}",
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

                //обновляем чайлды фолдера
                targetFileSystemParentFolder.AddChildFolder(
                    newFolder
                    );

                var result = new ActionFixer<IFolder>(
                    newFolder,
                    () =>
                    {
                        //fix this operation

                        //создаем папку
                        Directory.CreateDirectory(
                            targetAbsolutePath
                            );
                    },
                    () =>
                    {
                        //удаляем новую папку
                        //nothing to do
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

                var absolutePath = Path.Combine(
                    _fileSystem._preRootPath,
                    localFolder.FullPath
                    );

                var folderExists = Directory.Exists(absolutePath);

                if (!folderExists)
                {
                    throw new CHDException(
                        absolutePath,
                        CHDExceptionTypeEnum.FolderDoesNotExists
                        );
                }

                //обновляем чайлды фолдера
                var removedFolder = targetFileSystemParentFolder.RemoveChild(localFolder);

                var result = new ActionFixer<IFolder>(
                    removedFolder,
                    () =>
                    {
                        //fix this operation
                        if (Directory.Exists(absolutePath))
                        {
                            Directory.Delete(
                                absolutePath,
                                true
                                );
                        }
                    },
                    () =>
                    {
                        //move folder back
                        //nothing to do
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
                if (sourceFileSystemCopier == null)
                {
                    throw new ArgumentNullException("sourceFileSystemCopier");
                }
                if (sourceFileSystemFile == null)
                {
                    throw new ArgumentNullException("sourceFileSystemFile");
                }
                if (targetFileSystemParentFolder == null)
                {
                    throw new ArgumentNullException("targetFileSystemParentFolder");
                }

                //где надо создать файл
                var targetAbsolutePath = Path.Combine(
                    _fileSystem._preRootPath,
                    sourceFileSystemFile.FullPath
                    );

                //где временно скачаем файл? для быстрого выполнения операции file move при фиксации необходимо, чтобы скачанный файл
                //был на том же диске, что и исходное место расположение файла, но выше нашего синхронизируемого хранилища, чтобы изменения на ФС не вызывали 
                //снова сработки события "есть изменения в репозитории, начинаем синхронизацию"
                var targetBackupFilePath = Path.Combine(
                    _fileSystem._preRootPath,
                    Guid.NewGuid() + " " + sourceFileSystemFile.Name
                    );

                var oldFile = targetFileSystemParentFolder.GetFileByName(sourceFileSystemFile.Name);
                var fileExists = File.Exists(targetAbsolutePath);

                if ((oldFile != null && !fileExists) || (oldFile == null && fileExists))
                {
                    throw new CHDException(
                        string.Format(
                            "unknown problem with file {0}",
                            sourceFileSystemFile.FullPath
                            ),
                        CHDExceptionTypeEnum.UnknownFileProblem
                        );
                }

                //создаем новый файл
                var newFile = new StructureFile(
                    this._fileSystem.PathComparerProvider,
                    targetFileSystemParentFolder,
                    sourceFileSystemFile.Name,
                    sourceFileSystemFile.Size,
                    sourceFileSystemFile.LastWriteTimeUtc,
                    sourceFileSystemFile.ChangeIdentifier
                    );

                using (var destinationStream = new FileStream(targetBackupFilePath, FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    sourceFileSystemCopier.CopyFileTo(
                        sourceFileSystemFile,
                        destinationStream,
                        0,
                        sourceFileSystemFile.Size
                        );
                }

                //обновляем чайлды фолдера
                targetFileSystemParentFolder.CreateOrUpdateChildFile(
                    newFile
                    );

                var result = new ActionFixer<IFile>(
                    newFile,
                    () =>
                    {
                        //fix this operation

                        //удаляем оригинальный файл, если он есть
                        if (File.Exists(targetAbsolutePath))
                        {
                            File.Delete(
                                targetAbsolutePath
                                );
                        }

                        //переносим скачанный из бекапа на место исходного
                        File.Move(
                            targetBackupFilePath,
                            targetAbsolutePath
                            );

                        File.SetLastWriteTimeUtc(
                            targetAbsolutePath,
                            sourceFileSystemFile.LastWriteTimeUtc
                            );

                        //Debug.WriteLine(
                        //    "¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤ {0}: {1}",
                        //    targetAbsolutePath,
                        //    sourceFileSystemFile.LastWriteTimeUtc.Ticks
                        //    );
                    },
                    () =>
                    {
                        //move file back

                        //удаляем скачанный файл, он не пригодился
                        File.Delete(
                            targetBackupFilePath
                            );
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

                var absolutePath = Path.Combine(
                    _fileSystem._preRootPath,
                    localFile.FullPath
                    );

                var fileExists = File.Exists(absolutePath);

                if (!fileExists)
                {
                    throw new CHDException(
                        absolutePath,
                        CHDExceptionTypeEnum.FileNotFound
                        );
                }

                //обновляем чайлды фолдера
                var removedFile = targetFileSystemParentFolder.RemoveChild(localFile);

                var result = new ActionFixer<IFile>(
                    removedFile,
                    () =>
                    {
                        //fix this operation

                        //собственно, удаляем файл
                        if (File.Exists(absolutePath))
                        {
                            File.Delete(
                                absolutePath
                                );
                        }

                    },
                    () =>
                    {
                        //move file back
                        //nothing to do
                    }
                    );

                return
                    result;
            }
        }

        #endregion
    }
}