using System;
using CHD.Common;
using CHD.Common.ServiceCode.Executor;
using CHD.Email.Native;
using CHD.Email.Network.Imap;
using CHD.Email.Settings;

namespace CHD.Email.Network
{
    public sealed class EmailClientExecutor : INativeClientExecutor<EmailNativeMessage, EmailSendableMessage>
    {
        private readonly IImapClientFactory _imapClientFactory;
        private readonly IDisorderLogger _logger;

        public EmailClientExecutor(
            IImapClientFactory imapClientFactory,
            IDisorderLogger logger
            )
        {
            if (imapClientFactory == null)
            {
                throw new ArgumentNullException("imapClientFactory");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _imapClientFactory = imapClientFactory;
            _logger = logger;
        }

        public void Execute(
            Action<INativeClientEx<EmailNativeMessage, EmailSendableMessage>> executeAction
            )
        {
            if (executeAction == null)
            {
                throw new ArgumentNullException("executeAction");
            }

            using (var client = _imapClientFactory.CreateConnectedClient())
            {
                var clientex = new EmailNativeClientEx(client, _logger);

                executeAction(clientex);
            }
        }

        public T Execute<T>(
            Func<INativeClientEx<EmailNativeMessage, EmailSendableMessage>, T> executeAction
            )
        {
            if (executeAction == null)
            {
                throw new ArgumentNullException("executeAction");
            }

            using (var client = _imapClientFactory.CreateConnectedClient())
            {
                var clientex = new EmailNativeClientEx(client, _logger);

                var result = executeAction(clientex);
                
                return
                    result;
            }
        }
    }
}