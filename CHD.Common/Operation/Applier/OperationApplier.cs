using System;
using System.Collections.Generic;
using CHD.Common.FileSystem;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.Operation.FileOperation;
using CHD.Common.Operation.Fixer;
using CHD.Common.Operation.FolderOperation;

namespace CHD.Common.Operation.Applier
{
    public sealed class OperationApplier : IOperationApplier
    {
        private readonly IFileSystem _targetFileSystem;

        public OperationApplier(
            IFileSystem targetFileSystem
            )
        {
            if (targetFileSystem == null)
            {
                throw new ArgumentNullException("targetFileSystem");
            }

            _targetFileSystem = targetFileSystem;
        }

        public IOperationFixer ApplyOperation(
            IFileSystemCopier sourceFileSystemCopier,
            IFileOperation operation
            )
        {
            if (sourceFileSystemCopier == null)
            {
                throw new ArgumentNullException("sourceFileSystemCopier");
            }
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            var sourceFile = operation.File;

            var targetFolder = _targetFileSystem.RootFolder.GetDeepChildFolder(sourceFile.Folder);

            var fixers = new List<IFixer>();

            try
            {
                switch (operation.Type)
                {
                    case OperationTypeEnum.Create:
                    case OperationTypeEnum.Recreate:
                    {
                        var fixer = _targetFileSystem.Executor.CreateOrUpdateFile(sourceFileSystemCopier, sourceFile, targetFolder);
                        fixers.Add(fixer);

                        break;
                    }
                    case OperationTypeEnum.Delete:
                    {
                        var fixer = _targetFileSystem.Executor.DeleteFile(sourceFile, targetFolder);
                        fixers.Add(fixer);

                        break;
                    }
                    case OperationTypeEnum.NotSpecified:
                    default:
                        throw new ArgumentOutOfRangeException(operation.Type.ToString());
                }
            }
            catch
            {
                foreach (var fixer in fixers)
                {
                    fixer.SafelyRevert();
                }

                throw;
            }

            var result = new MultiOperationFixer(
                operation,
                fixers.ToArray()
                );

            return
                result;
        }

        public IOperationFixer ApplyOperation(
            IFileSystemCopier sourceFileSystemCopier,
            IFolderOperation operation
            )
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            var sourceFolder = operation.Folder;

            var targetParentFolder = _targetFileSystem.RootFolder.GetDeepChildFolder(sourceFolder.Parent);

            var fixers = new List<IFixer>();

            try
            {
                switch (operation.Type)
                {
                    case OperationTypeEnum.Create:
                    {
                        DoCreateFolder(
                            sourceFileSystemCopier,
                            sourceFolder,
                            targetParentFolder,
                            ref fixers
                            );

                        break;
                    }
                    case OperationTypeEnum.Recreate:
                    {
                        var deleteFixer = _targetFileSystem.Executor.DeleteChildFolder(sourceFolder, targetParentFolder);
                        fixers.Add(deleteFixer);

                        DoCreateFolder(
                            sourceFileSystemCopier,
                            sourceFolder, 
                            targetParentFolder, 
                            ref fixers
                            );

                        break;
                    }
                    case OperationTypeEnum.Delete:
                    {
                        var deleteFixer = _targetFileSystem.Executor.DeleteChildFolder(sourceFolder, targetParentFolder);
                        fixers.Add(deleteFixer);

                        break;
                    }
                    case OperationTypeEnum.NotSpecified:
                    default:
                        throw new ArgumentOutOfRangeException(operation.Type.ToString());
                }
            }
            catch
            {
                foreach (var fixer in fixers)
                {
                    fixer.SafelyRevert();
                }

                throw;
            }

            var result = new MultiOperationFixer(
                operation,
                fixers.ToArray()
                );

            return
                result;
        }

        //private void DoDeleteFolder(
        //    IFolder sourceFolder,
        //    IFolder targetParentFolder,
        //    ref List<IFixer> fixers
        //    )
        //{
        //    DoDeleteFilesRecursively(
        //        sourceFolder,
        //        targetParentFolder,
        //        ref fixers
        //        );

        //    var deleteFixer = _targetFileSystem.Executor.DeleteChildFolder(sourceFolder, targetParentFolder);
            
        //    fixers.Add(deleteFixer);
        //}

        //private void DoDeleteFilesRecursively(
        //    IFolder sourceFolder,
        //    IFolder targetFolder,
        //    ref List<IFixer> fixers
        //    )
        //{
        //    foreach (var file in sourceFolder.Files)
        //    {
        //        var fixer = _targetFileSystem.Executor.DeleteFile(file, targetFolder);
        //        fixers.Add(fixer);
        //    }

        //    foreach (var folder in sourceFolder.Folders)
        //    {
        //        var targetChildFolder = targetFolder.GetFolderByName(folder.Name);

        //        DoDeleteFilesRecursively(
        //            folder,
        //            targetChildFolder,
        //            ref fixers
        //            );
        //    }

        //}

        private void DoCreateFolder(
            IFileSystemCopier sourceFileSystemCopier,
            IFolder sourceFolder,
            IFolder targetParentFolder,
            ref List<IFixer> fixers
            )
        {
            var createFolderFixer = _targetFileSystem.Executor.CreateChildFolder(sourceFolder, targetParentFolder);
            fixers.Add(createFolderFixer);

            var createdFolder = createFolderFixer.Result;

            //create folder's files
            foreach (var file in sourceFolder.Files)
            {
                var createFileFixer = _targetFileSystem.Executor.CreateOrUpdateFile(
                    sourceFileSystemCopier,
                    file,
                    createdFolder
                    );

                fixers.Add(createFileFixer);
            }

            //create folder's folders
            foreach (var folder in sourceFolder.Folders)
            {
                //var targetFolder = createdFolder.GetFolderByName(folder.Name);

                DoCreateFolder(
                    sourceFileSystemCopier,
                    folder,
                    createdFolder,
                    ref fixers
                    );
            }
        }
    }
}