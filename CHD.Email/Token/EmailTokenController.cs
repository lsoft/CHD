using System;
using CHD.Common;
using CHD.Common.Native;
using CHD.Common.Saver;
using CHD.Common.ServiceCode.Executor;
using CHD.Token;
using CHD.Token.Releaser;

namespace CHD.Email.Token
{
    public sealed class EmailTokenController<TNativeMessage, TSendableMessage> : ITokenController
        where TNativeMessage : NativeMessage
        where TSendableMessage : SendableMessage
    {
        public const string TokenFolder = "$Token";

        private readonly IBackgroundReleaser _releaser;
        private readonly INativeClientExecutor<TNativeMessage, TSendableMessage> _executor;
        private readonly IDisorderLogger _logger;

        private readonly object _locker = new object();

        public EmailTokenController(
            IBackgroundReleaser releaser,
            INativeClientExecutor<TNativeMessage, TSendableMessage> executor,
            IDisorderLogger logger
            )
        {
            if (releaser == null)
            {
                throw new ArgumentNullException("releaser");
            }
            if (executor == null)
            {
                throw new ArgumentNullException("executor");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _releaser = releaser;
            _executor = executor;
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
            lock (_locker)
            {
                var result = _executor.Execute(
                    client => client.IsChildFolderExists(TokenFolder)
                    );

                return
                    result;
            }
        }

        private bool ReleaseToken()
        {
            lock (_locker)
            {
                string unused;
                var result = _executor.Execute(
                    client => client.DeleteChildFolder(TokenFolder, out unused)
                    );

                return result;
            }
        }

        private bool ObtainToken()
        {
            lock (_locker)
            {
                string unused;
                _executor.Execute(
                    client => client.CreateChildFolder(TokenFolder, out unused)
                    );

                return true;
            }
        }

    }
}