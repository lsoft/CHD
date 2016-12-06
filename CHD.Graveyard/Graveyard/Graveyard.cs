using System;
using System.Threading;
using CHD.Graveyard.ExclusiveAccess;
using CHD.Graveyard.ExclusiveAccess.Factory;
using CHD.Graveyard.Token;
using CHD.Graveyard.Token.Factory;

namespace CHD.Graveyard.Graveyard
{
    public class Graveyard : IGraveyard, IDisposable
    {
        private readonly IExclusiveAccessFactory _accessFactory;
        private readonly ITokenFactory _tokenFactory;

        private int _exclusiveAccessGiven = 0;
        
        private int _disposed = 0;

        public Graveyard(
            IExclusiveAccessFactory accessFactory,
            ITokenFactory tokenFactory
            )
        {
            if (accessFactory == null)
            {
                throw new ArgumentNullException("accessFactory");
            }
            if (tokenFactory == null)
            {
                throw new ArgumentNullException("tokenFactory");
            }

            _accessFactory = accessFactory;
            _tokenFactory = tokenFactory;
        }

        public bool TryGetExclusiveAccess(out IExclusiveAccess exclusiveAccess)
        {
            exclusiveAccess = null;
            var result = false;

            var tokenGiven = TakeGivenFlag();
            if (!tokenGiven)
            {
                return
                    result;
            }

            try
            {
                IToken token;
                if (!_tokenFactory.TryToObtainToken(out token))
                {
                    ReleaseGivenFlag();

                    throw new InvalidOperationException("TryToObtainToken");
                }

                try
                {
                    exclusiveAccess = _accessFactory.GetExclusiveAccess(
                        () =>
                        {
                            try
                            {
// ReSharper disable once AccessToDisposedClosure
                                token.Dispose();
                            }
                            finally
                            {
                                ReleaseGivenFlag();
                            }
                        }
                        );

                    result = true;
                }
                catch
                {
                    token.Dispose();
                    throw;
                }
            }
            catch
            {
                ReleaseGivenFlag();
            }

            return
                result;
        }


        public void Dispose()
        {
            var tokenGiven = _exclusiveAccessGiven;

            if (tokenGiven > 0)
            {
                throw new InvalidOperationException("tokenGiven > 0");
            }

            var disposed = Interlocked.Exchange(ref _disposed, 1);

            if (disposed == 0)
            {
                //nothing to do
            }
        }

        private bool TakeGivenFlag()
        {
            var tokenGiven = Interlocked.Exchange(ref _exclusiveAccessGiven, 1);

            return
                tokenGiven == 0;
        }

        private void ReleaseGivenFlag()
        {
            Interlocked.Exchange(ref _exclusiveAccessGiven, 0);
        }

    }
}
