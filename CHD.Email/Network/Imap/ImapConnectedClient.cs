using System;
using System.Threading;
using CHD.Common;
using CHD.Email.Settings;
using MailKit;
using MailKit.Net.Imap;

namespace CHD.Email.Network.Imap
{
    public sealed class ImapConnectedClient : IImapConnectedClient
    {
        private readonly object _locker = new object();

        private readonly IDisorderLogger _logger;

        public EmailSettings Settings
        {
            get;
            private set;
        }

        private int _disposed = 0;
        
        private MailKit.Net.Imap.ImapClient _client;
        private volatile IMailFolder _sent;

        public IMailFolder Sent
        {
            get
            {
                if (_sent == null)
                {
                    lock (_locker)
                    {
                        if (_sent == null)
                        {
                            _sent = _client.GetFolder(SpecialFolder.Sent);
                        }
                    }
                }

                _sent.Open(FolderAccess.ReadWrite);

                return
                    _sent;
            }
        }

        //public IMailFolder Trash
        //{
        //    get
        //    {
        //        IMailFolder result = null;

        //        if (_client != null && _client.IsConnected)
        //        {
        //            result = _client.GetFolder(SpecialFolder.Trash);
        //        }

        //        return
        //            result;
        //    }
        //}

        public IMailFolder SafelyGetChildfolder(
            IMailFolder parent,
            string folderName
            )
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName");
            }

            try
            {
                var result = DoGetSubfolder(
                    folderName
                    );

                if (result != null)
                {
                    result.Open(FolderAccess.ReadWrite);
                }

                return result;
            }
            catch (ImapCommandException)
            //catch (FolderNotFoundException)
            {
                //no folder, suppress this exception
            }

            return
                null;
        }

        public IMailFolder CreateFolder(
            IMailFolder parent,
            string folderName
            )
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName");
            }

            IMailFolder result = null;

            try
            {
                result = parent.Create(
                    folderName,
                    true
                    );
                
                result.Open(FolderAccess.ReadWrite);
            }
            catch (ImapCommandException)
            {
                //should nothing to do
            }

            return
                result;
        }

        public ImapConnectedClient(
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
        }

        public void Connect()
        {
            lock (_locker)
            {
                if (_client != null)
                {
                    throw new InvalidOperationException("_client != null");
                }

                var client = new MailKit.Net.Imap.ImapClient();

                try
                {
                    client.Connect(Settings.ImapServer, Settings.ImapPort, true);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    client.Authenticate(Settings.Email, Settings.Password);

                    _client = client;
                }
                catch
                {
                    client.Dispose();
                    throw;
                }
            }
        }

        public void Dispose()
        {
            lock (_locker)
            {
                var disposed = Interlocked.Exchange(ref _disposed, 1);
                if (disposed == 0)
                {
                    SafelyCleanup();
                }
            }
        }

        private void SafelyCleanup()
        {
            try
            {
                _client.Dispose();
            }
            catch(Exception excp)
            {
                _logger.LogException(excp);
            }
        }

        private IMailFolder DoGetSubfolder(string folderName)
        {
            try
            {
                var sf = Sent.GetSubfolder(
                    folderName
                    );

                return sf;
            }
            catch (FolderNotFoundException)
            {
                //no folder, suppress this exception
            }

            return null;
        }

    }
}