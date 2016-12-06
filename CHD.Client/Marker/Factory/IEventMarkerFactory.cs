using CHD.Graveyard.Marker;

namespace CHD.Client.Marker.Factory
{
    public interface IEventMarkerFactory : IMarkerFactory
    {
        event MarkerStatusChangedDelegate MarkerStatusChangedEvent;
    }
}