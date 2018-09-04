using System;
using System.IO;
using CHD.Common.FileSystem.FFile;
using CHD.Common.Letter;
using CHD.Common.Others;
using CHD.Common.Saver;
using CHD.Common.ServiceCode;
using CHD.MailRuCloud.Native;
using CHD.MailRuCloud.Settings;
using CHD.MailRuCloud.Structure;

namespace CHD.MailRuCloud.Letter
{
    public sealed class MailRuSendableMessageFactory : ISendableMessageFactory<MailRuSendableMessage>
    {

        public MailRuSendableMessageFactory(
            )
        {
        }

        public SendableWrapper<MailRuSendableMessage> CreateStructureMessage(
            string newSubject,
            byte[] attachmentData
            )
        {
            if (newSubject == null)
            {
                throw new ArgumentNullException("newSubject");
            }
            if (attachmentData == null)
            {
                throw new ArgumentNullException("attachmentData");
            }

            var sendableMessage = new MailRuSendableMessage(
                newSubject,
                attachmentData
                );

            var wrapper = new SendableWrapper<MailRuSendableMessage>(
                sendableMessage,
                () => { } //nothing to do
                );

            return
                wrapper;
        }

        public MailRuSendableMessage CreateRegularMessage(
            int structureCurrentVersion,
            long order,
            Guid transactionGuid,
            MessageTypeEnum operationType,
            ICopyableFile file,
            Stream stream
            )
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            var mailGuid = Guid.NewGuid();
            var subject = SubjectComposer.ComposeSubject(
                structureCurrentVersion,
                order,
                transactionGuid,
                operationType,
                mailGuid,
                file
                );

            MailRuSendableMessage result;

            if (stream != null)
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);

                    var data = ms.ToArray();

                    result = new MailRuSendableMessage(
                        subject,
                        data
                        );
                }
            }
            else
            {
                result = new MailRuSendableMessage(
                    subject,
                    new byte[0]
                    );
            }

            return result;
        }
    }


}