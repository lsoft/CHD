using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CHD.FileSystem.FileWrapper;
using CHD.Graveyard;
using CHD.Graveyard.Operation;

namespace CHD.Local.Operation
{
    public class LocalOperationFactory : IOperationFactory
    {
        private readonly string _folderPath;

        public LocalOperationFactory(
            string folderPath
            )
        {
            if (folderPath == null)
            {
                throw new ArgumentNullException("folderPath");
            }

            _folderPath = folderPath;

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        public List<IOperation> GetAllOperations(
            )
        {
            //scan folder
            var operations = (
                from f in Directory.GetFiles(_folderPath)
                let item = new LocalOperation(f)
                orderby item.Order ascending
                select item
                ).Cast<IOperation>().ToList();

            return
                operations;
        }

        public IOperation CreateBlockDataOperation(
            Guid transactionGuid,
            long nextOrder,
            IFileWrapper fileWrapper,
            byte[] data
            )
        {
            if (fileWrapper == null)
            {
                throw new ArgumentNullException("fileWrapper");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            var f = LocalOperation.CreateOperation(
                _folderPath,
                nextOrder,
                transactionGuid,
                GraveyardOperationTypeEnum.BlockData,
                fileWrapper,
                data
                );

            return f;
        }

        public IOperation CreateOpenFileOperation(
            Guid transactionGuid,
            long nextOrder,
            IFileWrapper fileWrapper
            )
        {
            if (fileWrapper == null)
            {
                throw new ArgumentNullException("fileWrapper");
            }

            var f = LocalOperation.CreateOperation(
                _folderPath,
                nextOrder,
                transactionGuid,
                GraveyardOperationTypeEnum.OpenFile,
                fileWrapper
                );

            return f;
        }

        public IOperation CreateCloseFileOperation(
            Guid transactionGuid,
            long nextOrder,
            IFileWrapper fileWrapper
            )
        {
            if (fileWrapper == null)
            {
                throw new ArgumentNullException("fileWrapper");
            }

            var f = LocalOperation.CreateOperation(
                _folderPath,
                nextOrder,
                transactionGuid,
                GraveyardOperationTypeEnum.CloseFile,
                fileWrapper
                );

            return f;
        }

        public IOperation CreateDeleteFileOperation(
            Guid transactionGuid,
            long nextOrder,
            IFileWrapper fileWrapper
            )
        {
            if (fileWrapper == null)
            {
                throw new ArgumentNullException("fileWrapper");
            }

            var f = LocalOperation.CreateOperation(
                _folderPath,
                nextOrder,
                transactionGuid,
                GraveyardOperationTypeEnum.DeleteFile,
                fileWrapper
                );

            return f;
        }
    }
}