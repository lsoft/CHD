using System;
using CHD.Graveyard.Marker;

namespace CHD.Graveyard.Token.Factory
{
    public class MarkerAndTokenController : ITokenController
    {
        private readonly IMarkerFactory _markerFactory;
        private readonly ITokenController _tokenController;

        public MarkerAndTokenController(
            IMarkerFactory markerFactory,
            ITokenController tokenController
            )
        {
            if (markerFactory == null)
            {
                throw new ArgumentNullException("markerFactory");
            }
            if (tokenController == null)
            {
                throw new ArgumentNullException("tokenController");
            }

            _markerFactory = markerFactory;
            _tokenController = tokenController;
        }

        public bool TryToObtainToken(out IToken token)
        {
            if (_markerFactory.IsMarkerCreated)
            {
                token = null;

                return
                    false;
            }

            _markerFactory.CreateMarker();
            try
            {
                IToken internalToken;
                var preResult = _tokenController.TryToObtainToken(out internalToken);

                if (!preResult)
                {
                    _markerFactory.SafelyDeleteMarker();

                    token = null;
                    return false;
                }

                token = new ActionToken(
                    () =>
                    {
                        try
                        {
                            internalToken.Dispose();
                        }
                        finally
                        {
                            _markerFactory.SafelyDeleteMarker();
                        }
                    });

                return
                    true;
            }
            catch
            {
                _markerFactory.SafelyDeleteMarker();
                throw;
            }
        }

        public bool TryToReleaseToken()
        {
            var result = _tokenController.TryToReleaseToken();
            
            if (result)
            {
                _markerFactory.SafelyDeleteMarker();
            }

            return result;
        }
    }
}