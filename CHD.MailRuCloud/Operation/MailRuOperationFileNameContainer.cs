using System;
using CHD.FileSystem.Algebra;
using CHD.Graveyard;

namespace CHD.MailRuCloud.Operation
{
    public class MailRuOperationFileNameContainer
    {
        public long Order
        {
            get;
            private set;
        }

        public Guid TransactionGuid
        {
            get;
            private set;
        }

        public GraveyardOperationTypeEnum OperationType
        {
            get;
            private set;
        }

        public Guid MailGuid
        {
            get;
            private set;
        }

        public Suffix FilePathSuffix
        {
            get;
            private set;
        }

        public string FileName
        {
            get;
            private set;
        }

        public MailRuOperationFileNameContainer(string fileName)
        {
            FileName = fileName;

            long order;
            Guid transactionGuid;
            GraveyardOperationTypeEnum operationType;
            Suffix filePathSuffix;
            Guid mailGuid;
            MailRuFileNameComposer.ParseFileName(
                fileName,
                out order,
                out transactionGuid,
                out operationType,
                out filePathSuffix,
                out mailGuid
                );

            Order = order;
            TransactionGuid = transactionGuid;
            OperationType = operationType;
            FilePathSuffix = filePathSuffix;
            MailGuid = mailGuid;
        }

        public static bool TryParse(
            string subject,
            out MailRuOperationFileNameContainer fileNameContainer
            )
        {
            var result = false;

            fileNameContainer = null;

            if (MailRuFileNameComposer.IsFileNameCanBeParsed(subject))
            {
                fileNameContainer = new MailRuOperationFileNameContainer(subject);
                result = true;
            }

            return result;
        }
    }
}
