using System;
using System.Collections.Generic;
using CHD.Common.ServiceCode;
using CHD.Email.Structure;
using CHD.Settings.Controller;

namespace CHD.Email.Settings
{
    public sealed class EmailSettings : IRemoteSettings, IVersionedSettings
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

        public long MaxFileBlockSize
        {
            get;
            private set;
        }

        //public long MinFileBlockSize
        //{
        //    get;
        //    private set;
        //}

        public int StoredSnapshotCount
        {
            get;
            private set;
        }

        public string StructureFolder
        {
            get;
            private set;
        }

        public string StructurePrefix
        {
            get;
            private set;
        }

        public EmailAddress StructureAddress
        {
            get;
            private set;
        }

        public EmailSettings(
            ISettings settings
            )
        {
            var actions = new Dictionary<string, Action<string>>(StringComparer.InvariantCultureIgnoreCase);
            actions.Add("Email", s => Email = s);
            actions.Add("Password", s => Password = s);
            actions.Add("ImapServer", s => ImapServer = s);
            actions.Add("ImapPort", s => ImapPort = int.Parse(s));
            actions.Add("MaxFileBlockSize", s => MaxFileBlockSize = long.Parse(s));
            //actions.Add("MinFileBlockSize", s => MinFileBlockSize = long.Parse(s));
            actions.Add("StoredSnapshotCount", s => StoredSnapshotCount = int.Parse(s));
            actions.Add("StructureFolder", s => StructureFolder = s);
            actions.Add("StructurePrefix", s => StructurePrefix = s);

            settings.Export(actions);

            StructureAddress = new EmailAddress(
                this.Email,
                this.StructureFolder,
                this.StructurePrefix
                );
        }
    }
}