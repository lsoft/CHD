using System;
using CHD.Common.FileSystem.FFile;
using CHD.Common.Saver;

namespace CHD.Email.Letter
{
    internal sealed class EmailBodyComposer
    {
        public static string ComposeBody(
            long order,
            Guid transactionGuid,
            MessageTypeEnum messageType,
            Guid mailGuid,
            INamedFile file
            )
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            var body = string.Join(
                Environment.NewLine, 
                order,
                transactionGuid,
                messageType,
                file.FullPath,
                mailGuid
                );

            return
                body;
        }
    }
}