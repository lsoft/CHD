namespace CHD.Email.Network.Imap
{
    public interface IImapClientFactory
    {
        IImapConnectedClient CreateConnectedClient(
            );
    }
}