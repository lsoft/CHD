using System;
using System.Collections.Generic;
using CHD.Settings.Controller;

namespace CHD.Client.Gui
{
    public sealed class AppSettings
    {


        public string DataChannelEndpoint
        {
            get;
            private set;
        }

        public AppSettings(
            ISettings settings
            )
        {
            var actions = new Dictionary<string, Action<string>>(StringComparer.InvariantCultureIgnoreCase);
            actions.Add("DataChannelEndpoint", s => DataChannelEndpoint = s);

            settings.Export(actions);
        }
    }
}