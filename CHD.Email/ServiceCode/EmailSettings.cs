using System;
using System.Collections.Generic;
using System.IO;
using CHD.Common;
using CHD.Settings;
using CHD.Settings.Controller;

namespace CHD.Email.ServiceCode
{
    public class EmailSettings : GraveyardSettings
    {
        public string Email
        {
            get;
            private set;
        }

        public string Password
        {
            get;
            private set;
        }

        public string ImapServer
        {
            get;
            private set;
        }

        public int ImapPort
        {
            get;
            private set;
        }


        public EmailSettings(
            ISettings settings
            )
            : base(settings)
        {
            var actions = new Dictionary<string, Action<string>>(StringComparer.InvariantCultureIgnoreCase);
            actions.Add("Email", s => Email = s);
            actions.Add("Password", s => Password = s);
            actions.Add("ImapServer", s => ImapServer = s);
            actions.Add("ImapPort", s => ImapPort = int.Parse(s));

            settings.Export(actions);
        }

        internal EmailSettings(string email, string password, string imapServer, int imapPort, long fileBlockSize)
            : base(fileBlockSize)
        {
            Email = email;
            Password = password;
            ImapServer = imapServer;
            ImapPort = imapPort;
        }
    }
}