using System;
using System.Threading;
using CHD.Common.Logger;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CHD.Email.ServiceCode
{
    internal class SmtpClientEx : IDisposable
    {
        private readonly IDisorderLogger _logger;
        private readonly SmtpClient _client;

        private int _disposed = 0;

        public SmtpClientEx(
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

            _logger = logger;

            var client = new SmtpClient();

            try
            {
                client.Connect(settings.SmtpServer, settings.SmtpPort, SecureSocketOptions.Auto);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(settings.Email, settings.Password);
            }
            catch
            {
                try
                {
                    client.Dispose();
                }
                catch(Exception excp)
                {
                    logger.LogException(excp);
                }

                throw;
            }

            _client = client;
        }

        public void Send(
            MimeMessage message
            )
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            _logger.LogFormattedMessage(
                "Message sending: {0}",
                message.Subject
                );
            try
            {
                _client.Send(message);

                _logger.LogFormattedMessage(
                    "Message sent successfully: {0}",
                    message.Subject
                    );
            }
            catch
            {
                _logger.LogFormattedMessage(
                    "Message send fails: {0}",
                    message.Subject
                    );

                throw;
            }
        }


        public void Dispose()
        {
            var disposed = Interlocked.Exchange(ref _disposed, 1);
            if (disposed == 0)
            {
                try
                {
                    _client.Disconnect(true);

                    _client.Dispose();
                }
                catch (Exception excp)
                {
                    _logger.LogException(excp);
                }
            }
        }
    }
}