using System;
using System.Collections.Generic;
using CHD.FileSystem.FileWrapper;

namespace CHD.Graveyard.Operation
{
    public interface IOperationFactory
    {
        List<IOperation> GetAllOperations(
            );

        IOperation CreateBlockDataOperation(
            Guid transactionGuid,
            long nextOrder,
            IFileWrapper fileWrapper,
            byte[] data
            );

        IOperation CreateOpenFileOperation(
            Guid transactionGuid,
            long nextOrder,
            IFileWrapper fileWrapper
            );

        IOperation CreateCloseFileOperation(
            Guid transactionGuid,
            long nextOrder,
            IFileWrapper fileWrapper
            );

        IOperation CreateDeleteFileOperation(
            Guid transactionGuid,
            long nextOrder,
            IFileWrapper fileWrapper
            );
    }
}