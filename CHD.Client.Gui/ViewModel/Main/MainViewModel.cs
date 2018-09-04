using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using CHD.Client.Gui.CompositionRoot.Helper;
using CHD.Client.Gui.DataFlow;
using CHD.Client.Gui.ViewModel.Main.Wrapper.Journal;
using CHD.Client.Gui.ViewModel.Main.Wrapper.Sync;
using CHD.WcfChannel;
using CHD.WcfChannel.Journal;
using CHD.Wpf;

namespace CHD.Client.Gui.ViewModel.Main
{
    public sealed class MainViewModel : BaseViewModel
    {
        private readonly IDataProvider _dataProvider;
        private readonly IDetailsWindowFactory _detailsWindowFactory;

        private string _watchFolder;
        private SyncWrapper _syncData;
        private JournalItemWrapper _selectedReport;

        public string WatchFolder
        {
            get
            {
                return
                    _watchFolder;
            }

            private set
            {
                if (_watchFolder == value)
                {
                    return;
                }

                _watchFolder = value;

                OnPropertyChanged("WatchFolder");
            }
        }

        public SyncWrapper SyncData
        {
            get
            {
                return
                    _syncData;
            }

            private set
            {
                if (_syncData == value)
                {
                    return;
                }

                _syncData = value;

                OnPropertyChanged("SyncData");
            }
        }


        public ObservableCollection2<JournalItemWrapper> JournalData
        {
            get;
            private set;
        }

        public JournalItemWrapper SelectedReport
        {
            get
            {
                return
                    _selectedReport;
            }

            set
            {
                _selectedReport = value;

                OnPropertyChanged("SelectedReport");
                OnCommandInvalidate();
            }
        }

        private ICommand _showDetailsCommand;
        public ICommand ShowDetailsCommand
        {
            get
            {
                if (_showDetailsCommand == null)
                {
                    _showDetailsCommand = new RelayCommand(
                        arg =>
                        {
                            var detailWindow = _detailsWindowFactory.Create(
                                SelectedReport
                                );
                            detailWindow.ShowDialog();
                        },
                        arg => SelectedReport != null
                        );
                }

                return
                    _showDetailsCommand;
            }
        }

        public MainViewModel(
            Dispatcher dispatcher,
            IDataProvider dataProvider,
            IDetailsWindowFactory detailsWindowFactory
            )
            : base(dispatcher)
        {
            if (dataProvider == null)
            {
                throw new ArgumentNullException("dataProvider");
            }
            if (detailsWindowFactory == null)
            {
                throw new ArgumentNullException("detailsWindowFactory");
            }

            _dataProvider = dataProvider;
            _detailsWindowFactory = detailsWindowFactory;

            JournalData = new ObservableCollection2<JournalItemWrapper>();

            Load(dataProvider);
        }

        private void Load(IDataProvider dataProvider)
        {
            dataProvider.CommonDataChangedEvent += DataProviderOnCommonDataChanged;
            DataProviderOnCommonDataChanged();

            dataProvider.SyncDataChangedEvent += DataProviderOnSyncDataChanged;
            DataProviderOnSyncDataChanged();

            dataProvider.JournalDataChangedEvent += DataProviderOnJournalDataChanged;
            DataProviderOnJournalDataChanged(dataProvider.JournalData.Wrappers);
        }

        public void Unload()
        {
            _dataProvider.CommonDataChangedEvent -= DataProviderOnCommonDataChanged;
            _dataProvider.SyncDataChangedEvent -= DataProviderOnSyncDataChanged;
            _dataProvider.JournalDataChangedEvent -= DataProviderOnJournalDataChanged;
        }

        
        private void DataProviderOnCommonDataChanged()
        {
            WatchFolder = _dataProvider.CommonData.WatchFolder;
        }

        private void DataProviderOnSyncDataChanged()
        {
            SyncData = _dataProvider.SyncData;
        }

        private void DataProviderOnJournalDataChanged(List<JournalItemWrapper> newJournalItems)
        {
            this._dispatcher.BeginInvoke(
                new Action(
                    () =>
                    {
                        JournalData.ReverseInsertAt0(
                            newJournalItems.OrderBy(j => j.Report.SyncDate)
                            );
                    })
                );
        }
    }
}
