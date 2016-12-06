using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHD.Common;
using CHD.Settings;
using CHD.Settings.Controller;

namespace CHD.MailRuCloud.ServiceCode
{
    public class MailRuSettings : GraveyardSettings
    {

        public string Login
        {
            get;
            private set;
        }

        public string Password
        {
            get;
            private set;
        }

        public MailRuSettings(
            ISettings settings
            )
            : base(settings)
        {
            var actions = new Dictionary<string, Action<string>>(StringComparer.InvariantCultureIgnoreCase);
            actions.Add("Login", s => Login = s);
            actions.Add("Password", s => Password = s);
            //actions.Add("ImapServer", s => ImapServer = s);
            //actions.Add("ImapPort", s => ImapPort = int.Parse(s));
            //actions.Add("SmtpServer", s => SmtpServer = s);
            //actions.Add("SmtpPort", s => SmtpPort = int.Parse(s));

            settings.Export(actions);
        }

        internal MailRuSettings(string login, string password, long fileBlockSize)
            : base(fileBlockSize)
        {
            Login = login;
            Password = password;
        }
    }
}
