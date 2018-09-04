using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using CHD.Common.Crypto;
using CHD.Settings.Controller;
using CHD.Wpf;

namespace CHD.Installer.ViewModel.Edit
{
    internal sealed class EditViewModel : BaseViewModel
    {
        private readonly ICrypto _realCrypto;
        private readonly ICrypto _fakeCrypto;
        private readonly ISettings _settings;
        private readonly KeyProvider _keyProvider;

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

        public ObservableCollection2<Record> Settings
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
                        j =>
                        {
                            byte[] unused;
                            return
                                _keyProvider.TryParseKey(this.EncodeSeed, out unused);
                        }
                        );
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
                        j => true
                        );
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
            string seed,
            KeyProvider keyProvider
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
            if (keyProvider == null)
            {
                throw new ArgumentNullException("keyProvider");
            }
            //seed allowed to be null

            _realCrypto = realCrypto;
            _fakeCrypto = fakeCrypto;
            _settings = settings;
            _keyProvider = keyProvider;

            CloseAction = closeAction;
            EncodeAllowed = seed != null;

            this.EncodeSeed = seed ?? string.Empty;

            Settings = new ObservableCollection2<Record>();
            Settings.AddRange(settings.Records.Select(record =>  new Record(record)));
        }

        private byte[] GetKey()
        {
            return
                _keyProvider.ParseKey(this.EncodeSeed);
        }

        private void UpdateSettings()
        {
            foreach (var s in Settings)
            {
                _settings.Update(s);
            }
        }
    }
}