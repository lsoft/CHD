using System;
using CHD.Common;
using CHD.Token.Container;

namespace CHD.Token
{
    public sealed class StatusTokenController : ITokenController
    {
        private readonly ITokenController _tokenController;
        private readonly ITokenContainer _tokenContainer;
        private readonly IDisorderLogger _logger;

        public StatusTokenController(
            ITokenController tokenController,
            ITokenContainer tokenContainer,
            IDisorderLogger logger
            )
        {
            if (tokenController == null)
            {
                throw new ArgumentNullException("tokenController");
            }
            if (tokenContainer == null)
            {
                throw new ArgumentNullException("tokenContainer");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _tokenController = tokenController;
            _tokenContainer = tokenContainer;
            _logger = logger;
        }

        public bool TryToObtainToken(out IToken token)
        {
            IToken preToken;
            var result = _tokenController.TryToObtainToken(out preToken);

            if (!result)
            {
                token = preToken;

                return
                    false;
            }

            //internal token has been taken
            //wrap it with a wrapper that changes a local token status

            _tokenContainer.UpdateTokenTakenStatus(true);

            token = new ActionToken(
                () =>
                {
                    SafelyReleaseToken(preToken);
                }
                );

            return
                true;
        }

        public bool TryToReleaseToken()
        {
            var result = _tokenController.TryToReleaseToken();

            if (result)
            {
                _tokenContainer.UpdateTokenTakenStatus(false);
            }

            return
                result;
        }

        private void SafelyReleaseToken(IToken token)
        {
            if (token == null)
            {
                return;
            }

            try
            {
                token.Dispose();

                _tokenContainer.UpdateTokenTakenStatus(false);
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);
            }
        }
    }
}