using System;
using System.Threading;
using CHD.Common;
using CHD.Common.Others;
using CHD.Common.ServiceCode.Executor;
using CHD.MailRuCloud.Native;
using CHD.MailRuCloud.Network;
using CHD.Token;
using CHD.Token.Releaser;

namespace CHD.MailRuCloud.Token
{
    public sealed class MailRuTokenController : ITokenController
    {
        public const string TokenFolder = "$Token";

        private readonly MailRuClientExecutor _executor;
        private readonly IBackgroundReleaser _releaser;
        private readonly IDisorderLogger _logger;

        public MailRuTokenController(
            MailRuClientExecutor executor,
            IBackgroundReleaser releaser,
            IDisorderLogger logger
            )
        {
            if (executor == null)
            {
                throw new ArgumentNullException("executor");
            }
            if (releaser == null)
            {
                throw new ArgumentNullException("releaser");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _executor = executor;
            _releaser = releaser;
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
            var result = false;

            try
            {
                ObtainToken();

                result = true;
            }
            catch (CHDException excp)
            {
                if (excp.ExceptionType.In(CHDExceptionTypeEnum.TokenCannotBeObtained, CHDExceptionTypeEnum.TokenCannotBeReleased))
                {
                    //suppress this type of the exception
                }
                else
                {
                    throw;
                }
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
            catch (CHDException excp)
            {
                if (excp.ExceptionType.In(CHDExceptionTypeEnum.TokenCannotBeObtained, CHDExceptionTypeEnum.TokenCannotBeReleased))
                {
                    //suppress this type of the exception
                }
                else
                {
                    throw;
                }
            }

            return
                result;
        }

        private bool IsTokenFree(
            )
        {
            var result = false;

            _executor.Execute(
                client => 
                {
                    result = !client.IsChildFolderExists(
                        TokenFolder
                        );
                });

            return
                result;
        }

        private void ReleaseToken()
        {
            _executor.Execute(
                client =>
                {
                    string deletedFolderName;
                    client.DeleteChildFolder(
                        TokenFolder,
                        out deletedFolderName
                        );

                    if (string.Compare(deletedFolderName, TokenFolder, StringComparison.InvariantCultureIgnoreCase) != 0)
                    {
                        throw new CHDException("Token cannot be released", CHDExceptionTypeEnum.TokenCannotBeReleased);
                    }
                });
        }

        private void ObtainToken()
        {
            _executor.Execute(
                client =>
                {
                    string createdFolderName;
                    client.CreateChildFolder(
                        TokenFolder,
                        out createdFolderName
                        );

                    if (string.Compare(createdFolderName, TokenFolder, StringComparison.InvariantCultureIgnoreCase) != 0)
                    {
                        string unused;
                        client.DeleteChildFolder(createdFolderName, out unused);

                        throw new CHDException("Token cannot be obtained", CHDExceptionTypeEnum.TokenCannotBeObtained);
                    }
                });
        }

    }
}