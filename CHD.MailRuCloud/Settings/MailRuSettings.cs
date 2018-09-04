using System;
using System.Collections.Generic;
using CHD.Common.ServiceCode;
using CHD.MailRuCloud.Structure;
using CHD.Settings.Controller;

namespace CHD.MailRuCloud.Settings
{
    public sealed class MailRuSettings : IRemoteSettings, IVersionedSettings
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

        public long MaxFileBlockSize
        {
            get;
            private set;
        }

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

        public MailRuAddress StructureAddress
        {
            get;
            private set;
        }

        public MailRuSettings(
            ISettings settings
            )
        {
            var actions = new Dictionary<string, Action<string>>(StringComparer.InvariantCultureIgnoreCase);
            actions.Add("Login", s => Login = s);
            actions.Add("Password", s => Password = s);
            actions.Add("MaxFileBlockSize", s => MaxFileBlockSize = long.Parse(s));
            //actions.Add("MinFileBlockSize", s => MinFileBlockSize = long.Parse(s));
            actions.Add("StoredSnapshotCount", s => StoredSnapshotCount = int.Parse(s));
            actions.Add("StructureFolder", s => StructureFolder = s);
            actions.Add("StructurePrefix", s => StructurePrefix = s);

            settings.Export(actions);

            StructureAddress = new MailRuAddress(
                this.StructureFolder,
                this.StructurePrefix
                );

        }
    }
}
