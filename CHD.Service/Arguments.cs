namespace CHD.Service
{
    public sealed class Arguments
    {
        public bool IsServiceMode
        {
            get;
            private set;
        }

        public string SettingsFile
        {
            get;
            private set;
        }

        public bool LocalClear
        {
            get;
            private set;
        }

        public bool RemoteClear
        {
            get;
            private set;
        }

        public Arguments(bool isServiceMode, string settingsFile, bool localClear, bool remoteClear)
        {
            IsServiceMode = isServiceMode;
            SettingsFile = settingsFile;
            LocalClear = localClear;
            RemoteClear = remoteClear;
        }
    }
}