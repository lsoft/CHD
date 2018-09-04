namespace CHD.Client.Gui.CompositionRoot
{
    public sealed class Arguments
    {
        public string SettingsFile
        {
            get;
            private set;
        }

        public Arguments(string settingsFile)
        {
            SettingsFile = settingsFile;
        }
    }
}