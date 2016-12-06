using System;
using CHD.Client.ViewModel.Components;
using CHD.Common.Logger;
using CHD.Graveyard.Marker;

namespace CHD.Client.Marker.Factory
{
    internal class EventMarkerFactory : IEventMarkerFactory, IMarkerFactory
    {
        private readonly IMarkerFactory _markerFactory;
        private readonly IDisorderLogger _logger;

        public bool IsMarkerCreated
        {
            get
            {
                return
                    _markerFactory.IsMarkerCreated;
            }
        }

        public EventMarkerFactory(
            IMarkerFactory markerFactory,
            IDisorderLogger logger
            )
        {
            if (markerFactory == null)
            {
                throw new ArgumentNullException("markerFactory");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _markerFactory = markerFactory;
            _logger = logger;
        }

        public void CreateMarker()
        {
            try
            {
                _markerFactory.CreateMarker();

                OnStatusChanged(true, null);
            }
            catch(Exception excp)
            {
                OnStatusChanged(true, excp);
                throw;
            }
        }

        public void SafelyDeleteMarker()
        {
            try
            {
                _markerFactory.SafelyDeleteMarker();

                OnStatusChanged(false, null);
            }
            catch (Exception excp)
            {
                OnStatusChanged(false, excp);
                throw;
            }
        }

        public event MarkerStatusChangedDelegate MarkerStatusChangedEvent;

        protected virtual void OnStatusChanged(
            bool taken,
            Exception exception
            )
        {
            try
            {
                var handler = MarkerStatusChangedEvent;
                if (handler != null)
                {
                    handler(
                        taken,
                        exception
                        );
                }
            }
            catch (Exception excp)
            {
                //any exception in notification code area should be logged and suppressed

                _logger.LogException(excp);
            }
        }
    }
}
