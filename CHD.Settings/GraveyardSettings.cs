using System;
using System.Collections.Generic;
using System.IO;
using CHD.Settings.Controller;

namespace CHD.Settings
{
    public class GraveyardSettings
    {
        protected readonly ISettings _settings;

        public long MaxFileBlockSize
        {
            get;
            private set;
        }
        public long MinFileBlockSize
        {
            get;
            private set;
        }
        
        public GraveyardSettings(
            long maxFileBlockSize,
            long minFileBlockSize
            )
        {
            MaxFileBlockSize = maxFileBlockSize;
            MinFileBlockSize = minFileBlockSize;
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
            actions.Add("MaxFileBlockSize", s => MaxFileBlockSize = long.Parse(s));
            actions.Add("MinFileBlockSize", s => MinFileBlockSize = long.Parse(s));

            settings.Export(actions);
        }

    }
}
