using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CHD.Common.Logger;
using CHD.Email.Operation;
using MailKit;
using MailKit.Net.Imap;
using MimeKit;

namespace CHD.Email.ServiceCode
{
    public class ImapClientEx : IDisposable
    {
        private readonly IDisorderLogger _logger;

        public EmailSettings Settings
        {
            get;
            private set;
        }

        private readonly ImapClient _client;

        private int _disposed = 0;

        public ImapClientEx(
            EmailSettings settings,
            IDisorderLogger logger
            )
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            Settings = settings;
            _logger = logger;

            _client = new ImapClient(
                settings,
                logger
                );

            try
            {
                _client.Connect();
            }
            catch
            {
                _client.Dispose();
                throw;
            }
        }

        public UniqueId? AppendMessage(
            MimeMessage message
            )
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            var sent = _client.Sent;

            var uid = sent.Append(
                message
                );

            return
                uid;
        }

        public void DeleteMessage(
            UniqueId uid
            )
        {
            _client.Sent.AddFlags(uid, MessageFlags.Deleted, true);
        }

        public bool IsSubfolderExists(string tokenFolder)
        {
            var result = false;

            try
            {
                var folder = _client.Sent.GetSubfolder(
                    tokenFolder
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

        public bool CreateFolder(string tokenFolder)
        {
            var result = false;

            try
            {
                var folder = _client.Sent.Create(
                    tokenFolder,
                    false
                    );

                result = true;
            }
            catch (ImapCommandException)
            {
                //should nothing to do
            }

            return
                result;
        }

        public bool DeleteFolder(string tokenFolder)
        {
            var result = false;

            try
            {
                var folder = _client.Sent.GetSubfolder(
                    tokenFolder
                    );

                if (folder != null)
                {
                    folder.Delete();

                    result = true;
                }
            }
            catch (ImapCommandException)
            {
                //should nothing to do
            }

            return
                result;
        }

        public List<IMessageSummary> Scan(
            )
        {
            var result = new List<IMessageSummary>();

            foreach (var summary in _client.Sent.Fetch(0, -1, MessageSummaryItems.UniqueId | MessageSummaryItems.Envelope))
            {
                var subject = summary.Envelope.Subject;
                if (EmailSubjectComposer.IsSubjectCanBeParsed(subject))
                {
                    result.Add(summary);
                }
            }

            return
                result;
        }

        public void DecodeAttachmentTo(
            UniqueId uid,
            Stream destination
            )
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            var message = _client.Sent.GetMessage(uid);

            var att = (MimePart)message.Attachments.First(j => j.IsAttachment);

            var before = destination.Position;

            att.ContentObject.DecodeTo(destination);

            var after = destination.Position;
            var bytes = after - before;
            _logger.LogFormattedMessage(
                "Decoded {0} bytes",
                bytes
                );
        }

        public void Dispose()
        {
            var disposed = Interlocked.Exchange(ref _disposed, 1);
            if (disposed == 0)
            {
                SafelyCleanup();
            }
        }

        private void SafelyCleanup()
        {
            try
            {
                _client.Dispose();
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);
            }
        }

    }
}