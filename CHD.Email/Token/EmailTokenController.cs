using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CHD.Common.Logger;
using CHD.Email.ServiceCode;
using CHD.Graveyard.Token;
using CHD.Graveyard.Token.Factory;
using CHD.Graveyard.Token.Releaser;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using ImapClient = CHD.Email.ServiceCode.ImapClient;

namespace CHD.Email.Token
{
    public class EmailTokenController : ITokenController
    {
        internal const string TokenFolder = "$Token";

        private readonly IBackgroundReleaser _releaser;
        private readonly EmailSettings _settings;
        private readonly IDisorderLogger _logger;

        public EmailTokenController(
            IBackgroundReleaser releaser,
            EmailSettings settings,
            IDisorderLogger logger
            )
        {
            if (releaser == null)
            {
                throw new ArgumentNullException("releaser");
            }
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _releaser = releaser;
            _settings = settings;
            _logger = logger;
        }

        public bool TryToObtainToken(
            out IToken token
            )
        {
            if (!TryToObtainTokenInternal())
            {
                token = null;
                return false;
            }

            token = new ActionToken(
                () => _releaser.SyncRelease(TryToReleaseTokenInternal)
                );

            return true;
        }

        public bool TryToReleaseToken()
        {
            return
                TryToReleaseTokenInternal();
        }

        private bool TryToObtainTokenInternal(
            )
        {
            var result = ObtainToken();

            return
                result;
        }

        private bool TryToReleaseTokenInternal()
        {
            var result = false;

            try
            {
                if (IsTokenFree())
                {
                    //token already free

                    return
                        true;
                }

                ReleaseToken();

                result = true;
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);
            }

            return
                result;
        }

        private bool IsTokenFree(
            )
        {
            var result = false;

            using (var client = new ImapClientEx(_settings, _logger))
            {
                result = client.IsSubfolderExists(
                    TokenFolder
                    );
            }

            return
                result;
        }

        private bool ReleaseToken()
        {
            var result = false;

            using (var client = new ImapClientEx(_settings, _logger))
            {
                result = client.DeleteFolder(
                    TokenFolder
                    );
            }
            return result;
        }

        private bool ObtainToken()
        {
            var result = false;

            using (var client = new ImapClientEx(_settings, _logger))
            {
                result = client.CreateFolder(
                    TokenFolder
                    );
            }

            return result;
        }

    }
}