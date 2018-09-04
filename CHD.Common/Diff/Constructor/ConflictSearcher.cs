using System;
using System.Collections.Generic;
using System.Linq;
using CHD.Common.Operation;
using CHD.Common.Operation.FileOperation;
using CHD.Common.Operation.FolderOperation;
using CHD.Common.Operation.Visitor;
using CHD.Common.Operation.Visitor.Splitter;
using CHD.Common.OperationLog;
using CHD.Common.Others;
using CHD.Common.PathComparer;

namespace CHD.Common.Diff.Constructor
{
    public sealed class ConflictSearcher : IConflictSearcher
    {
        public ConflictSearcher(
            )
        {
        }

        public IReadOnlyList<OperationPair> GetConflicts(
            IOperationLog localLog,
            IOperationLog remoteLog
            )
        {
            if (localLog == null)
            {
                throw new ArgumentNullException("localLog");
            }
            if (remoteLog == null)
            {
                throw new ArgumentNullException("remoteLog");
            }

            var conflicts = new List<OperationPair>();

            ScanLocalFoldersForRemoteChildChanges(localLog, remoteLog, ref conflicts);

            ScanRemoteFoldersForLocalChildChanges(localLog, remoteLog, ref conflicts);

            ScanLocalFileAgainstRemoteFile(localLog, remoteLog, ref conflicts);

            ScanRemoteFileAgainstLocalFile(localLog, remoteLog, ref conflicts);

            return
                conflicts;
        }


        #region private code

        private void ScanRemoteFoldersForLocalChildChanges(
            IOperationLog localLog,
            IOperationLog remoteLog,
            ref List<OperationPair> conflicts
            )
        {
            var remoteOperations = remoteLog.FolderOperations;

            foreach (var remoteOperation in remoteOperations)
            {
                var operation = remoteOperation; //to prevent access to modifier closure

                var childFolderOperations = localLog.FolderOperations.Where(
                    rfo => operation.IsByPathContains(rfo)
                    );
                var childFilesOperations = localLog.FileOperations.Where(
                    rfo => operation.IsByPathContains(rfo)
                    );

                var childItemOperations = new List<IOperation>();
                childItemOperations.AddRange(childFolderOperations);
                childItemOperations.AddRange(childFilesOperations);

                foreach (var childItemOperation in childItemOperations)
                {
                    //если одна и та же папка была удалена и локально и удаленно, то это не ошибка
                    if (remoteOperation.Type == OperationTypeEnum.Delete
                        && childItemOperation.Type == OperationTypeEnum.Delete
                        && childItemOperation.IsFolderOperation
                        )
                    {
                        var isSameTarget = remoteOperation.WithSameTarget(childItemOperation.FullPath);
                        if (isSameTarget)
                        {
                            continue;
                        }
                    }

                    //создание папки (с внутренней структурой!!!) и пересоздание является конфликтом
                    //возможно потом этот конфликт я придумаю как разрешать и уточню данный алгоритм

                    var conflict = new OperationPair(
                        remoteOperation,
                        childItemOperation
                        );

                    conflicts.Add(conflict);
                }
            }

        }

        private void ScanLocalFoldersForRemoteChildChanges(
            IOperationLog localLog,
            IOperationLog remoteLog,
            ref List<OperationPair> conflicts
            )
        {
            var localOperations = localLog.FolderOperations;


            foreach (var localOperation in localOperations)
            {
                var operation = localOperation; //to prevent access to modifier closure

                var childFolderOperations = remoteLog.FolderOperations.Where(
                    rfo => operation.IsByPathContains(rfo)
                    );
                var childFilesOperations = remoteLog.FileOperations.Where(
                    rfo => operation.IsByPathContains(rfo)
                    );

                var childItemOperations = new List<IOperation>();
                childItemOperations.AddRange(childFolderOperations);
                childItemOperations.AddRange(childFilesOperations);

                foreach (var childItemOperation in childItemOperations)
                {
                    //если одна и та же папка была удалена и локально и удаленно, то это не ошибка
                    if (localOperation.Type == OperationTypeEnum.Delete
                        && childItemOperation.Type == OperationTypeEnum.Delete
                        && childItemOperation.IsFolderOperation
                        )
                    {
                        var isSameTarget = localOperation.WithSameTarget(childItemOperation.FullPath);
                        if (isSameTarget)
                        {
                            continue;
                        }
                    }

                    //создание папки (с внутренней структурой!!!) и пересоздание является конфликтом
                    //возможно потом этот конфликт я придумаю как разрешать и уточню данный алгоритм

                    var conflict = new OperationPair(
                        localOperation,
                        childItemOperation
                        );

                    conflicts.Add(conflict);
                }
            }

        }

        private void ScanRemoteFileAgainstLocalFile(
            IOperationLog localLog,
            IOperationLog remoteLog,
            ref List<OperationPair> conflicts
            )
        {
            foreach (var remoteOperation in remoteLog.FileOperations)
            {
                IFileOperation localOperation;
                if (localLog.TryGetFileOperation(remoteOperation.File, out localOperation))
                {
                    //если один и тот же файл был удален и локально и удаленно, то это не ошибка
                    if (localOperation.Type == OperationTypeEnum.Delete && remoteOperation.Type == OperationTypeEnum.Delete)
                    {
                        continue;
                    }
                    
                    var conflict = new OperationPair(
                        localOperation,
                        remoteOperation
                        );

                    conflicts.Add(conflict);
                }
            }
        }

        private void ScanLocalFileAgainstRemoteFile(
            IOperationLog localLog,
            IOperationLog remoteLog,
            ref List<OperationPair> conflicts
            )
        {
            foreach (var localOperation in localLog.FileOperations)
            {
                IFileOperation remoteOperation;
                if (remoteLog.TryGetFileOperation(localOperation.File, out remoteOperation))
                {
                    //если один и тот же файл был удален и локально и удаленно, то это не ошибка
                    if (localOperation.Type == OperationTypeEnum.Delete && localOperation.Type == OperationTypeEnum.Delete)
                    {
                        continue;
                    }

                    var conflict = new OperationPair(
                        localOperation,
                        localOperation
                        );

                    conflicts.Add(conflict);
                }
            }
        }

        #endregion
    }

}