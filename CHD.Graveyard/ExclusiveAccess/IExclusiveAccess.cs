using System;
using System.Collections.Generic;
using CHD.FileSystem.FileWrapper;
using CHD.Graveyard.RemoteFile;
using CHD.Graveyard.RemoteFileState;

namespace CHD.Graveyard.ExclusiveAccess
{
    public interface IExclusiveAccess
    {
        bool ContainsTransaction(
            Guid transactionGuid
            );

        IRemoteFile OpenRemoteFile(
            Guid transactionGuid,
            IFileWrapper fileWrapper,
            long size
            );

        void DeleteRemoteFile(
            Guid transactionGuid,
            IFileWrapper fileWrapper
            );

        List<IRemoteFileState> GetSnapshotSince(
            long lastOrder
            );

        void Close(
            );
    }
}