using System;
using System.IO;
using System.Threading;
using CHD.Common.FileSystem.FFile;
using CHD.Common.Letter;
using CHD.Common.Native;
using CHD.Common.Others;
using CHD.Common.Saver;
using CHD.Common.ServiceCode;
using CHD.Email.Native;
using CHD.Email.Settings;
using CHD.Email.Structure;
using MailKit;
using MimeKit;

namespace CHD.Email.Letter
{
    public sealed class EmailSendableMessageFactory : ISendableMessageFactory<EmailSendableMessage>
    {
        private const string RegularMessageAttachmentName = "FileBodyAttachment";

        private readonly EmailSettings _emailSettings;

        public EmailSendableMessageFactory(
            EmailSettings emailSettings
            )
        {
            if (emailSettings == null)
            {
                throw new ArgumentNullException("emailSettings");
            }

            _emailSettings = emailSettings;
        }

        public SendableWrapper<EmailSendableMessage> CreateStructureMessage(
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

            var mimeMessage = new MimeMessage();

            var ms = Base64Helper.EncodeToStream(attachmentData);

            try
            {
                mimeMessage.From.Add(new MailboxAddress(string.Empty, _emailSettings.Email));
                mimeMessage.To.Add(new MailboxAddress(string.Empty, _emailSettings.Email));

                mimeMessage.Subject = newSubject;

                var textBody = new TextPart("plain")
                {
                    Text = string.Empty
                };

                var emailBody = new Multipart("mixed");
                emailBody.Add(textBody);

                // create an attachment
                var attachmentBody = new MimePart("application", "octet-stream")
                {
                    ContentObject = new ContentObject(ms, ContentEncoding.Base64),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = "Body"
                };

                emailBody.Add(attachmentBody);

                mimeMessage.Body = emailBody;

                var sendableMessage = new EmailSendableMessage(
                    mimeMessage
                    );

                var wrapper = new SendableWrapper<EmailSendableMessage>(
                    sendableMessage,
                    () => ms.Dispose()
                    );

                return
                    wrapper;
            }
            catch
            {
                ms.Dispose();
                throw;
            }
        }

        public EmailSendableMessage CreateRegularMessage(
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
            var subject = string.Empty;
            UniqueId? uid = null;

            var mimeMessage = new MimeMessage();

            #region message composition

            mimeMessage.From.Add(new MailboxAddress(string.Empty, _emailSettings.Email));
            mimeMessage.To.Add(new MailboxAddress(string.Empty, _emailSettings.Email));

            subject = SubjectComposer.ComposeSubject(
                structureCurrentVersion,
                order,
                transactionGuid,
                operationType,
                mailGuid,
                file
                );

            mimeMessage.Subject = subject;

            var body = EmailBodyComposer.ComposeBody(
                order,
                transactionGuid,
                operationType,
                mailGuid,
                file
                );

            var textBody = new TextPart("plain")
            {
                Text = body
            };

            var emailBody = new Multipart("mixed");
            emailBody.Add(textBody);

            if (stream != null)
            {
                // create an attachment
                var attachmentBody = new MimePart("application", "octet-stream")
                {
                    ContentObject = new ContentObject(stream, ContentEncoding.Binary),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = RegularMessageAttachmentName
                };

                emailBody.Add(attachmentBody);
            }

            mimeMessage.Body = emailBody;

            #endregion

            var sendableMessage = new EmailSendableMessage(
                mimeMessage
                );

            return sendableMessage;
        }
    }


}