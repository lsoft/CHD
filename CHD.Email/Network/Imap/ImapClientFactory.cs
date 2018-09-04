using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using CHD.Common;
using CHD.Common.Others;
using CHD.Email.Settings;

namespace CHD.Email.Network.Imap
{
    public sealed class ImapClientFactory : IImapClientFactory
    {
        private readonly EmailSettings _settings;
        private readonly IDisorderLogger _logger;

        public ImapClientFactory(
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
            _settings = settings;
            _logger = logger;
        }

        public IImapConnectedClient CreateConnectedClient()
        {
            var result = new ImapConnectedClient(
                _settings,
                _logger
                );

            try
            {
                result.Connect();
            }
            catch
            {
                result.Dispose();
                throw;
            }

            return
                result;
        }
    }
}