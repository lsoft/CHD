using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using CHD.Common;
using CHD.Common.Crypto;
using CHD.Installer.CompositionRoot.Components;
using CHD.Installer.Scanner;
using CHD.Settings.Controller;
using CHD.Wpf;

namespace CHD.Installer.ViewModel
{
    internal sealed class MainViewModel : BaseViewModel
    {
        private readonly IEditWindowFactory _editWindowFactory;
        private readonly ISettingsFactory _settingsFactory;
        private readonly ICrypto _realCrypto;
        private readonly ICrypto _fakeCrypto;
        //private readonly KeyProvider _keyProvider;
        private readonly IDisorderLogger _logger;

        public string EncodeSeed
        {
            get;
            set;
        }

        public string MainSettingsFilePath
        {
            get;
            set;
        }

        public string EmailSettingsFilePath
        {
            get;
            set;
        }

        public string MailRuCloudSettingsFilePath
        {
            get;
            set;
        }

        private ICommand _editMainSettingsCommand;
        public ICommand EditMainSettingsCommand
        {
            get
            {
                if (_editMainSettingsCommand == null)
                {
                    _editMainSettingsCommand = new RelayCommand(
                        j =>
                        {
                            ShowWindow(
                                MainSettingsFilePath,
                                null
                                );

                            OnPropertyChanged(string.Empty);
                        },
                        j => true);
                }

                return
                    _editMainSettingsCommand;
            }
        }

        private ICommand _editEmailSettingsCommand;
        public ICommand EditEmailSettingsCommand
        {
            get
            {
                if (_editEmailSettingsCommand == null)
                {
                    _editEmailSettingsCommand = new RelayCommand(
                        j =>
                        {
                            ShowWindow(
                                EmailSettingsFilePath,
                                this.EncodeSeed
                                );

                            OnPropertyChanged(string.Empty);
                        },
                        j => true);
                }

                return
                    _editEmailSettingsCommand;
            }
        }

        private ICommand _editMailRuCloudSettingsCommand;
        public ICommand EditMailRuCloudSettingsCommand
        {
            get
            {
                if (_editMailRuCloudSettingsCommand == null)
                {
                    _editMailRuCloudSettingsCommand = new RelayCommand(
                        j =>
                        {
                            ShowWindow(
                                MailRuCloudSettingsFilePath,
                                this.EncodeSeed
                                );

                            OnPropertyChanged(string.Empty);
                        },
                        j => true
                        );
                }

                return
                    _editMailRuCloudSettingsCommand;
            }
        }


        public MainViewModel(
            Dispatcher dispatcher,
            SettingsFileScanner mainSettingsScanner,
            SettingsFileScanner emailSettingsScanner,
            SettingsFileScanner mailRuCloudSettingsScanner,
            IEditWindowFactory editWindowFactory,
            ISettingsFactory settingsFactory,
            ICrypto realCrypto,
            ICrypto fakeCrypto,
            //KeyProvider keyProvider,
            IDisorderLogger logger
            )
            : base(dispatcher)
        {
            if (mainSettingsScanner == null)
            {
                throw new ArgumentNullException("mainSettingsScanner");
            }
            if (emailSettingsScanner == null)
            {
                throw new ArgumentNullException("emailSettingsScanner");
            }
            if (mailRuCloudSettingsScanner == null)
            {
                throw new ArgumentNullException("mailRuCloudSettingsScanner");
            }
            if (editWindowFactory == null)
            {
                throw new ArgumentNullException("editWindowFactory");
            }
            if (settingsFactory == null)
            {
                throw new ArgumentNullException("settingsFactory");
            }
            if (realCrypto == null)
            {
                throw new ArgumentNullException("realCrypto");
            }
            if (fakeCrypto == null)
            {
                throw new ArgumentNullException("fakeCrypto");
            }
            //if (keyProvider == null)
            //{
            //    throw new ArgumentNullException("keyProvider");
            //}
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _editWindowFactory = editWindowFactory;
            _settingsFactory = settingsFactory;
            _realCrypto = realCrypto;
            _fakeCrypto = fakeCrypto;
            //_keyProvider = keyProvider;
            _logger = logger;

            EncodeSeed = string.Empty;

            string msf;
            if (mainSettingsScanner.TryToFindSettingsFile(out msf))
            {
                this.MainSettingsFilePath = msf;
            }
            else
            {
                this.MainSettingsFilePath = string.Empty;
            }

            string esf;
            if (emailSettingsScanner.TryToFindSettingsFile(out esf))
            {
                this.EmailSettingsFilePath = esf;
            }
            else
            {
                this.EmailSettingsFilePath = string.Empty;
            }

            string mrcsf;
            if (mailRuCloudSettingsScanner.TryToFindSettingsFile(out mrcsf))
            {
                this.MailRuCloudSettingsFilePath = mrcsf;
            }
            else
            {
                this.MailRuCloudSettingsFilePath = string.Empty;
            }
        }

        //public ICrypto GetCrypto(string encodeSeed)
        //{
        //    if (string.IsNullOrEmpty(encodeSeed))
        //    {
        //        return
        //            _fakeCrypto;
        //    }

        //    var key = _keyProvider.ParseKey(encodeSeed);

        //    if (key == null)
        //    {
        //        return
        //            _fakeCrypto;
        //    }

        //    _realCrypto.LoadKey(key);

        //    return
        //        _realCrypto;
        //}

        private void ShowWindow(
            string settingsFilePath,
            string encodeSeed
            )
        {
            if (settingsFilePath == null)
            {
                throw new ArgumentNullException("settingsFilePath");
            }

            try
            {
                var settings = _settingsFactory.LoadSettings(
                    settingsFilePath
                    //,GetCrypto(encodeSeed)
                    );

                var ew = _editWindowFactory.Create(
                    settings,
                    encodeSeed
                    );

                ew.ShowDialog();
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);

                MessageBox.Show(
                    string.Format("{0}{2}{1}", excp.Message, Environment.NewLine, excp.StackTrace),
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );
            }
        }
    }
}
