using System;
using System.Linq;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.Operation;
using CHD.Common.Operation.FileOperation;
using CHD.Common.Operation.FolderOperation;
using CHD.Common.PathComparer;

namespace CHD.Common.OperationLog
{
    public sealed class OperationLogGenerator : IOperationLogGenerator
    {
        private readonly IPathComparerProvider _pathComparerProvider;

        public OperationLogGenerator(
            IPathComparerProvider pathComparerProvider
            )
        {
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }

            _pathComparerProvider = pathComparerProvider;
        }

        public IOperationLog Generate(
            IFolder previous,
            IFolder next
            )
        {
            if (previous == null)
            {
                throw new ArgumentNullException("previous");
            }
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }

            if (ReferenceEquals(previous, next))
            {
                return
                    OperationLog.Empty;
            }
            
            //root itself should not change!

            var result = new OperationLog();

            ProcessFolder(
                previous,
                next,
                ref result
                );

            return
                result;
        }

        private void ProcessFolder(
            IFolder previous, 
            IFolder next,
            ref OperationLog log
            )
        {
            if (previous == null)
            {
                throw new ArgumentNullException("previous");
            }
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }

            #region process files in this folder

            var pFiles = previous.Files;
            var nFiles = next.Files;

            var commonFiles = pFiles.Intersect(nFiles, _pathComparerProvider.GetFileComparer());

            //��� ����� ���� � ��� � ���; �������� ��� ��������� ��� � ��� ���� ��������� - ���������� ChangeIdentifier
            foreach (var pFile in commonFiles)
            {
                var nFile = next.GetFileByName(pFile);

                var pChangeIdentifier = pFile.ChangeIdentifier;
                var nChangeIdentifier = nFile.ChangeIdentifier;

                if (pChangeIdentifier != nChangeIdentifier)
                {
                    //���� ��������� � �����

                    var operation = new FileOperation(
                        OperationTypeEnum.Recreate,
                        nFile
                        );

                    log.AddOperation(
                        operation
                        );
                }
            }

            var pDeletedFiles = pFiles.Except(nFiles, _pathComparerProvider.GetFileComparer());
            foreach (var pDeletedFile in pDeletedFiles)
            {
                //��� ����� ���� �������

                var operation = new FileOperation(
                    OperationTypeEnum.Delete,
                    pDeletedFile
                    );

                log.AddOperation(
                    operation
                    );
            }

            var nCreatedFiles = nFiles.Except(pFiles, _pathComparerProvider.GetFileComparer());
            foreach (var nCreatedFile in nCreatedFiles)
            {
                //��� ����� ���� �������

                var operation = new FileOperation(
                    OperationTypeEnum.Create,
                    nCreatedFile
                    );

                log.AddOperation(
                    operation
                    );
            }

            #endregion

            #region process folders

            var pFolders = previous.Folders;
            var nFolders = next.Folders;

            var pCommonFolders = pFolders.Intersect(nFolders, _pathComparerProvider.GetFolderComparer());
            //��� ����� ���� � ��� � ���; �������� ��� ��������� ��� � ��� ���� ��������� - ���������� ChangeIdentifier
            foreach (var pFolder in pCommonFolders)
            {
                var nFolder = next.GetFolderByName(pFolder.Name);

                var pChangeIdentifier = pFolder.ChangeIdentifier;
                var nChangeIdentifier = nFolder.ChangeIdentifier;

                if (pChangeIdentifier != nChangeIdentifier)
                {
                    //���� ��������� � ����� - ��� ��������, ��� ������ ���� �������, � ����� - �������

                    ////������� �������� �������� ������ �����
                    //var deleteFolderOperation = new FolderOperation(
                    //    OperationTypeEnum.Delete,
                    //    pFolder
                    //    );

                    //log.AddOperation(
                    //    deleteFolderOperation
                    //    );

                    //������� �������� ������������ ����� �����
                    var createFolderOperation = new FolderOperation(
                        OperationTypeEnum.Recreate,
                        nFolder
                        );

                    log.AddOperation(
                        createFolderOperation
                        );
                }
                else
                {
                    //��������� � ����� ����� ���, �������� ���� ��������� � �� �������
                    //���������� ����������

                    ProcessFolder(
                        pFolder,
                        nFolder,
                        ref log
                        );
                }
            }


            var pDeletedFolders = pFolders.Except(nFolders, _pathComparerProvider.GetFolderComparer());
            foreach (var pDeletedFolder in pDeletedFolders)
            {
                //��� ����� ���� �������

                var operation = new FolderOperation(
                    OperationTypeEnum.Delete,
                    pDeletedFolder
                    );

                log.AddOperation(
                    operation
                    );
            }

            var nCreatedFolders = nFolders.Except(pFolders, _pathComparerProvider.GetFolderComparer());
            foreach (var nCreatedFolder in nCreatedFolders)
            {
                //��� ����� ���� �������

                var operation = new FolderOperation(
                    OperationTypeEnum.Create,
                    nCreatedFolder
                    );

                log.AddOperation(
                    operation
                    );
            }

            #endregion
        }
    }
}