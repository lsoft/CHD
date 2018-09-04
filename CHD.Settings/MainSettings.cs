using System;
using System.Collections.Generic;
using System.IO;
using CHD.Settings.Controller;
using CHD.Settings.Mode;

namespace CHD.Settings
{
    public sealed class MainSettings
    {
        private static readonly IDictionary<string, ModeEnum> _modeDictionary = new Dictionary<string, ModeEnum>(StringComparer.InvariantCultureIgnoreCase);

        private readonly ISettings _settings;

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

        //public TimeSpan LiveTime
        //{
        //    get;
        //    private set;
        //}

        //public TimeSpan DelayTime
        //{
        //    get;
        //    private set;
        //}

        //public string GraveyardSettingsFile
        //{
        //    get;
        //    private set;
        //}

        //public string PermanentStoreFile
        //{
        //    get;
        //    private set;
        //}

        //public string KeyValueFolder
        //{
        //    get;
        //    private set;
        //}

        //public int PullTimeoutAfterSuccessTimeoutMsec
        //{
        //    get;
        //    private set;
        //}

        //public int PullTimeoutAfterFailureTimeoutMsec
        //{
        //    get;
        //    private set;
        //}

        //public int PushTimeoutAfterFailureMsec
        //{
        //    get;
        //    private set;
        //}

        //public string MarkerHistory
        //{
        //    get;
        //    private set;
        //}

        //public bool IsSettingsEncoded
        //{
        //    get;
        //    private set;
        //}

        public MainSettings(
            ISettings settings
            )
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            _settings = settings;

            var actions = new Dictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase);
            actions.Add("Mode", s => Mode = _modeDictionary[s]);
            actions.Add("WatchFolder", s => WatchFolder = s);
            //actions.Add("LiveTime", s => LiveTime = TimeSpan.FromMilliseconds(int.Parse(s)));
            //actions.Add("DelayTime", s => DelayTime = TimeSpan.FromMilliseconds(int.Parse(s)));
            //actions.Add("GraveyardSettingsFile", s => GraveyardSettingsFile = s);
            //actions.Add("PermanentStoreFile", s => PermanentStoreFile = s);
            //actions.Add("KeyValueFolder", s => KeyValueFolder = s);
            //actions.Add("PullTimeoutAfterSuccessTimeoutMsec", s => PullTimeoutAfterSuccessTimeoutMsec = int.Parse(s));
            //actions.Add("PullTimeoutAfterFailureTimeoutMsec", s => PullTimeoutAfterFailureTimeoutMsec = int.Parse(s));
            //actions.Add("PushTimeoutAfterFailureMsec", s => PushTimeoutAfterFailureMsec = int.Parse(s));
            //actions.Add("MarkerHistory", s => MarkerHistory = s);
            //actions.Add("IsSettingsEncoded", s => IsSettingsEncoded = bool.Parse(s));

            settings.Export(actions);
        }

        static MainSettings()
        {
            _modeDictionary.Add(ModeEnum.Disk.ToString(), ModeEnum.Disk);
            _modeDictionary.Add(ModeEnum.Email.ToString(), ModeEnum.Email);
            _modeDictionary.Add("cloud.mail.ru", ModeEnum.CloudMailru);
        }
    }
}
