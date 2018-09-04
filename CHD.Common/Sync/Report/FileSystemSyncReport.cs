using System;
using System.Collections.Generic;
using CHD.Common.Operation;
using CHD.Common.Operation.FileOperation;
using CHD.Common.Operation.FolderOperation;
using CHD.Common.Operation.Visitor;
using CHD.Common.Serializer;

namespace CHD.Common.Sync.Report
{
    [Serializable]
    internal sealed class FileSystemSyncReport : IFileSystemSyncReport, IOperationVisitor, IOperationDumper
    {
        public SerializationVersionProvider<FileSystemSyncReport> SerializationVersion = new SerializationVersionProvider<FileSystemSyncReport>();

        private readonly List<SingleOperationReport> _operations = new List<SingleOperationReport>();

        public int TotalTouched
        {
            get
            {
                return
                    FilesTouchedCount + FoldersTouchedCount;
            }
        }

        public int FilesTouchedCount
        {
            get
            {
                return
                    DeletedFilesCount + CreatedFilesCount + RecreatedFilesCount;
            }
        }

        public int DeletedFilesCount
        {
            get;
            private set;
        }

        public int CreatedFilesCount
        {
            get;
            private set;
        }

        public int RecreatedFilesCount
        {
            get;
            private set;
        }

        public int FoldersTouchedCount
        {
            get
            {
                return
                    DeletedFoldersCount + CreatedFoldersCount + RecreatedFoldersCount;
            }
        }

        public int DeletedFoldersCount
        {
            get;
            private set;
        }

        public int CreatedFoldersCount
        {
            get;
            private set;
        }

        public int RecreatedFoldersCount
        {
            get;
            private set;
        }

        public IReadOnlyList<SingleOperationReport> Operations
        {
            get
            {
                return
                    _operations;
            }
        }

        public void Dump(IDisorderLogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            logger.LogFormattedMessage("TotalTouched: {0}", TotalTouched);
            if (TotalTouched > 0)
            {
                logger.LogFormattedMessage("  FilesTouchedCount: {0}", FilesTouchedCount);
                if (FilesTouchedCount > 0)
                {
                    logger.LogFormattedMessage("    DeletedFilesCount: {0}", DeletedFilesCount);
                    logger.LogFormattedMessage("    CreatedFilesCount: {0}", CreatedFilesCount);
                    logger.LogFormattedMessage("    RecreatedFilesCount: {0}", RecreatedFilesCount);
                }

                logger.LogFormattedMessage("  FoldersTouchedCount: {0}", FoldersTouchedCount);
                if (FoldersTouchedCount > 0)
                {
                    logger.LogFormattedMessage("    DeletedFoldersCount: {0}", DeletedFoldersCount);
                    logger.LogFormattedMessage("    CreatedFoldersCount: {0}", CreatedFoldersCount);
                    logger.LogFormattedMessage("    RecreatedFoldersCount: {0}", RecreatedFoldersCount);
                }
            }
        }

        public FileSystemSyncReport()
        {
            DeletedFilesCount = 0;
            CreatedFilesCount = 0;
            RecreatedFilesCount = 0;

            DeletedFoldersCount = 0;
            CreatedFoldersCount = 0;
            RecreatedFoldersCount = 0;
        }

        public void Visit(IFileOperation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            switch (operation.Type)
            {
                case OperationTypeEnum.Create:
                    CreatedFilesCount++;
                    break;
                case OperationTypeEnum.Recreate:
                    RecreatedFilesCount++;
                    break;
                case OperationTypeEnum.Delete:
                    DeletedFilesCount++;
                    break;
                case OperationTypeEnum.NotSpecified:
                default:
                    throw new ArgumentOutOfRangeException(operation.Type.ToString());
            }

            operation.Dump(
                this
                );
        }

        public void Visit(IFolderOperation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            switch (operation.Type)
            {
                case OperationTypeEnum.Create:
                    CreatedFoldersCount += operation.FolderCount;
                    CreatedFilesCount += operation.FileCount;
                    break;
                case OperationTypeEnum.Recreate:
                    RecreatedFoldersCount += operation.FolderCount;
                    RecreatedFilesCount += operation.FileCount;
                    break;
                case OperationTypeEnum.Delete:
                    DeletedFoldersCount += operation.FolderCount;
                    DeletedFilesCount += operation.FileCount;
                    break;
                case OperationTypeEnum.NotSpecified:
                default:
                    throw new ArgumentOutOfRangeException(operation.Type.ToString());
            }

            operation.Dump(
                this
                );
        }

        public void LogOperation(
            OperationTypeEnum type,
            string fullPath
            )
        {
            if (fullPath == null)
            {
                throw new ArgumentNullException("fullPath");
            }

            var op = new SingleOperationReport(type, fullPath);
            _operations.Add(op);
        }





        public static IFileSystemSyncReport Empty
        {
            get;
            private set;
        }

        static FileSystemSyncReport()
        {
            Empty = new FileSystemSyncReport();
        }

    }
}