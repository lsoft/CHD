using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CHD.Common;
using CHD.Common.Logger;
using CHD.Email.ServiceCode;
using CHD.Email.Token;
using CHD.FileSystem.Algebra;
using CHD.FileSystem.FileWrapper;
using CHD.Graveyard;
using CHD.Graveyard.Operation;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Search;
using MimeKit;

namespace CHD.Email.Operation
{
    [DebuggerDisplay("Order = {Order} {OperationType} {FilePathSuffix}")]
    public class EmailOperation : IOperation
    {
        private const string AttachmentName = "BinaryAttachment";

        private readonly EmailSettings _settings;
        private readonly UniqueId _uid;

        private readonly Guid _mailGuid;
        private readonly IDisorderLogger _logger;

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

        public Suffix FilePathSuffix
        {
            get;
            private set;
        }

        public EmailOperation(
            EmailSettings settings,
            UniqueId uid,
            string subject,
            IDisorderLogger logger
            )
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (subject == null)
            {
                throw new ArgumentNullException("subject");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (!uid.IsValid)
            {
                throw new ArgumentException("!uid.IsValid");
            }

            _settings = settings;
            _uid = uid;
            _logger = logger;

            long order;
            Guid transactionGuid;
            GraveyardOperationTypeEnum operationType;
            Suffix filePathSuffix;
            Guid mailGuid;
            EmailSubjectComposer.ParseSubject(
                subject,
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
            _mailGuid = mailGuid;
        }

        internal static EmailOperation CreateOperation(
            IDisorderLogger logger,
            EmailSettings settings,
            long order,
            Guid transactionGuid,
            GraveyardOperationTypeEnum operationType,
            IFileWrapper fileWrapper,
            byte[] data = null
            )
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (fileWrapper == null)
            {
                throw new ArgumentNullException("fileWrapper");
            }

            var mailGuid = Guid.NewGuid();
            var subject = string.Empty;
            UniqueId? uid = null;

            var message = new MimeMessage();
            
            #region compose message and then send the message to email server

            using (var ms = (data != null ? Base64Helper.EncodeToStream(data) : null))
            {
                #region message composition

                message.From.Add(new MailboxAddress(string.Empty, settings.Email));
                message.To.Add(new MailboxAddress(string.Empty, settings.Email));

                subject = EmailSubjectComposer.ComposeSubject(
                    order,
                    transactionGuid,
                    operationType,
                    mailGuid,
                    fileWrapper
                    );

                message.Subject = subject;

                var body = EmailBodyComposer.ComposeBody(
                    order,
                    transactionGuid,
                    operationType,
                    mailGuid,
                    fileWrapper
                    );

                var textBody = new TextPart("plain")
                {
                    Text = body
                };

                var emailBody = new Multipart("mixed");
                emailBody.Add(textBody);

                if (data != null)
                {
                    // create an attachment
                    var attachmentBody = new MimePart("application", "octet-stream")
                    {
                        ContentObject = new ContentObject(ms, ContentEncoding.Base64),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = AttachmentName
                    };
                    
                    emailBody.Add(attachmentBody);
                }

                message.Body = emailBody;

                #endregion

                //append the email to Sent folder
                using (var client = new ImapClientEx(settings, logger))
                {
                    uid = client.AppendMessage(
                        message
                        );
                }
            }

            #endregion

            if (!uid.HasValue || uid == UniqueId.Invalid)
            {
                throw new InvalidOperationException("uid invalid");
            }

            var operation = new EmailOperation(
                settings,
                uid.Value,
                subject,
                logger
                );

            return
                operation;
        }

        public void WriteRemoteDataTo(
            Stream destination
            )
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            if (OperationType != GraveyardOperationTypeEnum.BlockData)
            {
                throw new InvalidOperationException("OperationType != GraveyardOperationTypeEnum.BlockData");
            }

            using (var client = new ImapClientEx(_settings, _logger))
            {
                client.DecodeAttachmentTo(
                    _uid,
                    destination
                    );
            }
        }

        public void Delete()
        {
            DoDelete(
                );
        }

        private void DoDelete(
            )
        {
            using (var client = new ImapClientEx(_settings, _logger))
            {
                client.DeleteMessage(_uid);
            }
        }
    }
}
