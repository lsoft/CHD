using System;
using System.Collections.Generic;
using System.IO;
using CHD.Settings.Controller;
using CHD.Settings.Mode;

namespace CHD.Service
{
    public sealed class ServiceSettings
    {
        private static readonly IDictionary<string, ModeEnum> _modeDictionary = new Dictionary<string, ModeEnum>(StringComparer.InvariantCultureIgnoreCase);

        private readonly List<string> _skipFolders = new List<string>();

        public ModeEnum Mode
        {
            get;
            private set;
        }

        public string WatchFolder
        {
            get;
            private set;
        }

        public string PreRootPath
        {
            get
            {
                var di = new DirectoryInfo(WatchFolder);
                var pdi = di.Parent;

                return
                    pdi.FullName;
            }
        }

        public string WatchFolderName
        {
            get
            {
                var di = new DirectoryInfo(WatchFolder);

                return
                    di.Name;
            }
        }

        public IReadOnlyList<string> SkipFolders
        {
            get
            {
                return
                    _skipFolders;
            }
        }

        public string RemoteFileSystemSettingsFile
        {
            get;
            private set;
        }

        public string PluginFolder
        {
            get;
            private set;
        }

        public bool IsPluginsExists
        {
            get
            {
                return
                    !string.IsNullOrEmpty(PluginFolder);
            }
        }

        public string PropertiesFolder
        {
            get;
            private set;
        }

        public int ScheduledScanTimeoutMsec
        {
            get;
            private set;
        }

        public string StructureFilePath
        {
            get;
            private set;
        }

        public string SyncJournalFile
        {
            get;
            private set;
        }

        public int SyncReportStoreDays
        {
            get;
            private set;
        }

        public string DataChannelEndpoint
        {
            get;
            private set;
        }

        public ServiceSettings(
            ISettings settings
            )
        {
            var actions = new Dictionary<string, Action<string>>(StringComparer.InvariantCultureIgnoreCase);
            actions.Add("Mode", s => Mode = _modeDictionary[s]);
            actions.Add("WatchFolder", s => WatchFolder = s);
            actions.Add("SkipFolders", s => _skipFolders.Add(s));
            actions.Add("RemoteFileSystemSettingsFile", s => RemoteFileSystemSettingsFile = s);
            actions.Add("PluginFolder", s => PluginFolder = s);
            actions.Add("PropertiesFolder", s => PropertiesFolder = s);
            actions.Add("ScheduledScanTimeoutMsec", s => ScheduledScanTimeoutMsec = int.Parse(s));
            actions.Add("StructureFilePath", s => StructureFilePath = s);
            actions.Add("SyncJournalFile", s => SyncJournalFile = s);
            actions.Add("SyncReportStoreDays", s => SyncReportStoreDays = int.Parse(s));
            actions.Add("DataChannelEndpoint", s => DataChannelEndpoint = s);

            settings.Export(actions);
        }

        static ServiceSettings()
        {
            _modeDictionary.Add(ModeEnum.Disk.ToString(), ModeEnum.Disk);
            _modeDictionary.Add(ModeEnum.Email.ToString(), ModeEnum.Email);
            _modeDictionary.Add("cloud.mail.ru", ModeEnum.CloudMailru);
        }
    }
}