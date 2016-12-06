using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Windows.Threading;
using CHD.Client.Marker.History;
using CHD.Client.View;
using CHD.Client.ViewModel.Components;
using CHD.Wpf;

namespace CHD.Client.ViewModel
{
    internal class MarkerHistoryViewModel : BaseViewModel
    {
        private readonly IViewChanger _viewChanger;

        public ObservableCollection2<RecordWrapper> Records
        {
            get;
            private set;
        }

        private ICommand _backCommand;
        public ICommand BackCommand
        {
            get
            {
                if (_backCommand == null)
                {
                    _backCommand = new RelayCommand(
                        j =>
                        {
                            _viewChanger.ChangeForm(typeof(CurrentStatusView));

                            OnPropertyChanged(string.Empty);
                        },
                        j => true);
                }

                return
                    _backCommand;
            }
        }

        public MarkerHistoryViewModel(
            Dispatcher dispatcher,
            IViewChanger viewChanger,
            IRecordContainer recordContainer
            ) : base(dispatcher)
        {
            if (viewChanger == null)
            {
                throw new ArgumentNullException("viewChanger");
            }
            if (recordContainer == null)
            {
                throw new ArgumentNullException("recordContainer");
            }

            _viewChanger = viewChanger;

            Records = new ObservableCollection2<RecordWrapper>();

            recordContainer.StatusChangedEvent += RecordContainerOnStatusChanged;
        }

        private void RecordContainerOnStatusChanged(List<Record> newRecords)
        {
            this.BeginInvoke(
                () =>
                {
                    Records.ReverseInsertAt0(
                         newRecords.ConvertAll(j => new RecordWrapper(j))
                        );

                    //OnPropertyChanged(string.Empty);
                });
        }
    }
}