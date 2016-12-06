using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using CHD.Common;
using CHD.Common.Crypto;
using CHD.Settings;
using CHD.Settings.Controller;
using CHD.Wpf;

namespace CHD.Installer.ViewModel
{
    internal class EditViewModel : BaseViewModel
    {
        private readonly ICrypto _realCrypto;
        private readonly ICrypto _fakeCrypto;
        private readonly ISettings _settings;

        public string WindowTitle
        {
            get
            {
                return
                    "Редактирование настроек";
            }
        }

        public string EncodeSeed
        {
            get;
            set;
        }

        public ObservableCollection2<ItemWrapper> Settings
        {
            get;
            private set;
        }

        public bool EncodeAllowed
        {
            get;
            private set;
        }

        private ICommand _encodeAndSaveCommand;
        public ICommand EncodeAndSaveCommand
        {
            get
            {
                if (_encodeAndSaveCommand == null)
                {
                    _encodeAndSaveCommand = new RelayCommand(
                        j =>
                        {
                            UpdateSettings();

                            _realCrypto.LoadKey(GetKey());

                            _settings.Rewrite(_realCrypto);

                            CloseAction();

                            OnPropertyChanged(string.Empty);
                        },
                        j => true);
                }

                return
                    _encodeAndSaveCommand;
            }
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(
                        j =>
                        {
                            UpdateSettings();

                            _settings.Rewrite(_fakeCrypto);

                            CloseAction();

                            OnPropertyChanged(string.Empty);
                        },
                        j => true);
                }

                return
                    _saveCommand;
            }
        }

        public Action CloseAction
        {
            get;
            private set;
        }

        public EditViewModel(
            Dispatcher dispatcher,
            ICrypto realCrypto,
            ICrypto fakeCrypto,
            ISettings settings,
            Action closeAction,
            string seed
            )
            : base(dispatcher)
        {
            if (realCrypto == null)
            {
                throw new ArgumentNullException("realCrypto");
            }
            if (fakeCrypto == null)
            {
                throw new ArgumentNullException("fakeCrypto");
            }
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (closeAction == null)
            {
                throw new ArgumentNullException("closeAction");
            }
            //seed allowed to be null

            _realCrypto = realCrypto;
            _fakeCrypto = fakeCrypto;
            _settings = settings;

            CloseAction = closeAction;
            EncodeAllowed = seed != null;

            this.EncodeSeed = seed ?? string.Empty;

            Settings = new ObservableCollection2<ItemWrapper>();
            Settings.AddRange(settings.Records.Select(j => new ItemWrapper(j)));
        }

        private byte[] GetKey()
        {
            return
                KeyProvider.ProvideKey(this.EncodeSeed);
        }

        private void UpdateSettings()
        {
            foreach (var s in Settings)
            {
                _settings.Update(s);
            }
        }

        public class ItemWrapper : ISettingRecordInner
        {
            public string Name
            {
                get;
                private set;
            }

            private string _value;
            public string Value
            {
                get
                {
                    return
                        _value;
                }

                set
                {
                    _value = value;
                }
            }

            public string Comment
            {
                get;
                private set;
            }

            public ObservableCollection2<Option> Values
            {
                get;
                private set;
            }

            public ItemWrapper(
                ISettingRecord sr
                )
            {
                if (sr == null)
                {
                    throw new ArgumentNullException("sr");
                }

                Name = sr.Name;
                Value = sr.Value;
                Comment = sr.Comment;

                Values = new ObservableCollection2<Option>();
                Values.Add(new Option(sr.Value, true, false));
                if (!string.IsNullOrEmpty(sr.PreferredValue))
                {
                    Values.Add(new Option(sr.PreferredValue, false, true));
                }
                Values.AddRange(sr.Values.ConvertAll(j => new Option(j, false, false)));
            }

            public class Option
            {
                public string Value
                {
                    get;
                    private set;
                }

                public bool IsInUse
                {
                    get;
                    private set;
                }

                public bool IsPreferred
                {
                    get;
                    private set;
                }

                public string FullValue
                {
                    get
                    {
                        return
                            string.Format(
                                "{0}{1}{2}",
                                Value,
                                IsInUse ? " [актуальное значение]" : string.Empty,
                                IsPreferred ? " [предпочтительное значение]" : string.Empty
                                );
                    }
                }

                public Option(string value, bool isInUse, bool isPreferred)
                {
                    Value = value;
                    IsInUse = isInUse;
                    IsPreferred = isPreferred;
                }

                public override string ToString()
                {
                    return
                        Value;
                }
            }
        }

    }
}