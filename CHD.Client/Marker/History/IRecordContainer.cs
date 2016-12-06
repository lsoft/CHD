namespace CHD.Client.Marker.History
{
    internal interface IRecordContainer
    {
        event RecordContainerChangedDelegate StatusChangedEvent;

        void Prepare();
    }
}