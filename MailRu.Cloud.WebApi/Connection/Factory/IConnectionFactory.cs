namespace MailRu.Cloud.WebApi.Connection.Factory
{
    public interface IConnectionFactory
    {
        IConnection OpenConnection(
            string login,
            string password
            );
    }
}