namespace CHD.Graveyard.RemoteFile
{
    public interface IRemoteFile
    {
        void StoreBlock(
            byte[] data
            );

        void Close(
            );
    }
}