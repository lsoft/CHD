using System;
using CHD.Common;
using CHD.Common.Logger;
using CHD.Graveyard;
using CHD.Graveyard.Token;
using CHD.Graveyard.Token.Factory;
using CHD.Graveyard.Token.Releaser;
using CHD.MailRuCloud.ServiceCode;

namespace CHD.MailRuCloud.Token
{
    public class MailRuTokenController : ITokenController
    {
        internal const string TokenFolder = "$Token";

        private readonly IBackgroundReleaser _releaser;
        private readonly MailRuSettings _settings;
        private readonly IDisorderLogger _logger;

        public MailRuTokenController(
            IBackgroundReleaser releaser,
            MailRuSettings settings,
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
                () => _releaser.TryToReleaseAtBackgroundThread(TryToReleaseTokenInternal)
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
            var result = false;

            try
            {
                ObtainToken();

                result = true;
            }
            catch (TokenException)
            {
                //suppress this type of the exception
            }

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
            catch (TokenException)
            {
                //suppress this type of the exception
            }

            return
                result;
        }

        private bool IsTokenFree(
            )
        {
            var result = false;

            using (var client = new MailRuClientEx(_settings, _logger))
            {
                result = !client.IsSubfolderExists(
                    TokenFolder
                    );
            }

            return
                result;
        }

        private void ReleaseToken()
        {
            using (var client = new MailRuClientEx(_settings, _logger))
            {
                var deletedFolderPath = client.DeleteFolder(
                    TokenFolder
                    );

                if (string.Compare(deletedFolderPath, TokenFolder, StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    throw new TokenException("Token cannot be released", TokenActionEnum.Release);
                }
            }
        }

        private void ObtainToken()
        {
            using (var client = new MailRuClientEx(_settings, _logger))
            {
                var createdFolderPath = client.CreateFolder(
                    TokenFolder
                    );

                if (string.Compare(createdFolderPath, TokenFolder, StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    client.DeleteFolder(createdFolderPath);

                    throw new TokenException("Token cannot be obtained", TokenActionEnum.Obtain);
                }
            }
        }

    }
}