namespace CHD.Graveyard.Marker
{
    public interface IMarkerFactory
    {
        bool IsMarkerCreated
        {
            get;
        }

        void CreateMarker();

        void SafelyDeleteMarker();
    }
}