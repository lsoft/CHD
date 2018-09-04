namespace MailRu.Cloud.WebApi.Connection.Factory
{
    public interface IMailRuConnectionFactory
    {
        IMailRuConnection OpenConnection(
            string login,
            string password
            );
    }
}