namespace CHD.Client.Gui.DataFlow.Retriever
{
    public interface IDataRetriever
    {
        void AsyncStart();
        void SyncStop();
    }
}