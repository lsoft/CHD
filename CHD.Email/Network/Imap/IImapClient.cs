using MailKit;

namespace CHD.Email.Network.Imap
{
    public interface IImapClient
    {
        IMailFolder Sent
        {
            get;
        }

        IMailFolder SafelyGetChildfolder(
            IMailFolder parent,
            string folderName
            );

        IMailFolder CreateFolder(
            IMailFolder parent,
            string folderName
            );
    }
}