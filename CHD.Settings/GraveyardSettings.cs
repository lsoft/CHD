using System;
using System.Collections.Generic;
using System.IO;
using CHD.Settings.Controller;

namespace CHD.Settings
{
    public class GraveyardSettings
    {
        protected readonly ISettings _settings;

        public long FileBlockSize
        {
            get;
            private set;
        }

        public GraveyardSettings(
            long fileBlockSize
            )
        {
            FileBlockSize = fileBlockSize;
        }

        public GraveyardSettings(
            ISettings settings
            )
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            _settings = settings;


            var actions = new Dictionary<string, Action<string>>(StringComparer.InvariantCultureIgnoreCase);
            actions.Add("FileBlockSize", s => FileBlockSize = long.Parse(s));

            settings.Export(actions);
        }

    }
}
