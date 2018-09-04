using System;
using System.IO;
using CHD.Common.FileSystem.FFile;
using CHD.Common.Native;
using CHD.Common.Saver;

namespace CHD.Common.Letter
{
    public interface ISendableMessageFactory<TSendableMessage>
        where TSendableMessage : SendableMessage
    {
        SendableWrapper<TSendableMessage> CreateStructureMessage(
            string newSubject,
            byte[] attachmentData
            );

        TSendableMessage CreateRegularMessage(
            int structureCurrentVersion,
            long order,
            Guid transactionGuid,
            MessageTypeEnum operationType,
            ICopyableFile file,
            Stream stream
            );
    }
}