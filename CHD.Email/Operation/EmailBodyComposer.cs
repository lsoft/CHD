using System;
using System.Globalization;
using CHD.FileSystem.FileWrapper;
using CHD.Graveyard;

namespace CHD.Email.Operation
{
    internal class EmailBodyComposer
    {
        public static string ComposeBody(
            long order,
            Guid transactionGuid,
            GraveyardOperationTypeEnum operationType,
            Guid mailGuid,
            IFileWrapper fileWrapper
            )
        {
            var body = string.Join(
                Environment.NewLine, 
                order,
                transactionGuid,
                operationType,
                fileWrapper.FilePathSuffix.FilePathSuffix,
                mailGuid
                );

            return
                body;
        }
    }
}