using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CHD.Common;
using CHD.Common.Others;
using CHD.Common.ServiceCode;
using CHD.Common.ServiceCode.Executor;
using CHD.Email.Native;
using CHD.Email.Network.Imap;
using CHD.Email.Settings;
using MailKit;
using MailKit.Net.Imap;
using MimeKit;

namespace CHD.Email.Network
{

    public sealed class EmailNativeClientEx : INativeClientEx<EmailNativeMessage, EmailSendableMessage>
    {
        private readonly IDisorderLogger _logger;

        private readonly IImapClient _client;
        private readonly IMailFolder _folder;


        public EmailNativeClientEx(
            IImapClient client,
            IDisorderLogger logger
            )
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _client = client;
            _folder = client.Sent;
            _logger = logger;
        }

        private EmailNativeClientEx(
            IImapClient client,
            IMailFolder folder,
            IDisorderLogger logger
            )
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }
            if (folder == null)
            {
                throw new ArgumentNullException("folder");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _client = client;
            _folder = folder;
            _logger = logger;
        }

        public bool TryGetChild(
            string folderName,
            out INativeClientEx<EmailNativeMessage, EmailSendableMessage> client
            )
        {
            var result = false;
            client = null;

            try
            {
                var folder = _client.SafelyGetChildfolder(
                    _folder,
                    folderName
                    );

                if (folder != null)
                {
                    result = true;

                    client = new EmailNativeClientEx(
                        _client,
                        folder,
                        _logger
                        );
                }
            }
            catch (ImapCommandException)
            {
                //should nothing to do
            }

            return
                result;
        }

        public INativeClientEx<EmailNativeMessage, EmailSendableMessage> CreateOrEnterChild(
            string folderName
            )
        {
            INativeClientEx<EmailNativeMessage, EmailSendableMessage> result = null;

            try
            {
                var folder = _client.SafelyGetChildfolder(
                    _folder,
                    folderName
                    );

                if (folder == null)
                {
                    folder = DoCreateFolder(folderName);
                }


                result = new EmailNativeClientEx(
                    _client,
                    folder,
                    _logger
                    );
            }
            catch (ImapCommandException)
            {
                //should nothing to do
            }

            return
                result;
        }

        public void StoreMessage(
            EmailSendableMessage message
            )
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            DoStore(message, _folder);
        }

        private void DoStore(
            EmailSendableMessage message,
            IMailFolder folder
            )
        {
            var uid = folder.Append(
                message.Message
                );

            _logger.LogFormattedMessage(
                "Message '{1}' sent to folder '{0}'",
                folder.Name,
                message.Message.Subject
                );
        }

        public void DeleteMessages(
            IEnumerable<EmailNativeMessage> messages
            )
        {
            if (messages == null)
            {
                throw new ArgumentNullException("messages");
            }

            var uids = messages.Select(j => j.Summary.UniqueId).Distinct().ToArray();
            if (uids.Length > 0)
            {
                _folder.AddFlags(uids, MessageFlags.Deleted, true);
            }
        }

        public bool IsChildFolderExists(string folderName)
        {
            var result = false;

            try
            {
                var folder = _client.SafelyGetChildfolder(
                    _folder,
                    folderName
                    );

                result = (folder == null);
            }
            catch (ImapCommandException)
            {
                //should nothing to do
            }

            return
                result;
        }

        public void CreateChildFolder(
            string folderName,
            out string createdFolderName
            )
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName");
            }

            var folder = DoCreateFolder(folderName);

            if (folder == null)
            {
                throw new CHDException(
                    string.Format("Cannot create folder {0}", folderName),
                    CHDExceptionTypeEnum.CannotCreateFolder
                    );
            }

            createdFolderName = folder.Name;
        }

        public bool DeleteChildFolder(string folderName, out string deletedFolderName)
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName");
            }

            var result = false;
            deletedFolderName = null;

            try
            {
                var folder = _client.SafelyGetChildfolder(
                    _folder,
                    folderName
                    );

                if (folder != null)
                {
                    folder.Delete();

                    deletedFolderName = folder.Name;
                    result = true;

                    _logger.LogFormattedMessage(
                        "Folder '{0}' deleted",
                        folderName
                        );
                }
            }
            catch (ImapCommandException)
            {
                //should nothing to do
            }

            return
                result;
        }

        public List<EmailNativeMessage> ReadAndFilterMessages(
            Func<string, bool> filter
            )
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }

            var messages = new List<EmailNativeMessage>();

            try
            {
                foreach (var summary in _folder.Fetch(0, -1, MessageSummaryItems.UniqueId | MessageSummaryItems.Envelope))
                {
                    var subject = summary.Envelope.Subject;

                    if (filter(subject))
                    {
                        messages.Add(new EmailNativeMessage(summary));
                    }
                }

                return messages;
            }
            catch (ImapCommandException)
            {
                //should nothing to do
            }

            return null;
        }

        public long DecodeAttachmentTo(
            EmailNativeMessage message,
            Stream destination,
            long position = 0,
            long size = 0
            )
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }
            if (!destination.CanWrite)
            {
                throw new ArgumentException("!destination.CanWrite");
            }

            var result = DoDecodeAttachmentTo(_folder, message, destination, position, size);

            return
                result;
        }

        private long DoDecodeAttachmentTo(
            IMailFolder folder,
            EmailNativeMessage nativeMessage,
            Stream destination,
            long position,
            long size
            )
        {
            if (folder == null)
            {
                throw new ArgumentNullException("folder");
            }
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }
            if (!destination.CanWrite)
            {
                throw new ArgumentException("!destination.CanWrite");
            }

            var message = folder.GetMessage(nativeMessage.Summary.UniqueId);

            var att = (MimePart)message.Attachments.First(j => j.IsAttachment);

            using (var tempbuffer = new MemoryStream())
            {
                att.ContentObject.DecodeTo(tempbuffer);

                if (size == 0)
                {
                    size = tempbuffer.Position;
                }

                _logger.LogFormattedMessage(
                    "Decoded {0} bytes from message '{1}'",
                    tempbuffer.Position,
                    message.Subject
                    );

                tempbuffer.Position = position;

                var copiedSize = tempbuffer.CopyToConstraint(destination, size);

                _logger.LogFormattedMessage(
                    "Copied {0} bytes from message '{1}'",
                    copiedSize,
                    message.Subject
                    );

                return copiedSize;
            }
        }

        private IMailFolder DoCreateFolder(string folderName)
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName");
            }

            var result = _client.CreateFolder(
                _folder,
                folderName
                );

            return
                result;
        }
    }
}