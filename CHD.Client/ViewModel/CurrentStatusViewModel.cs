using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using CHD.Client.FileOperation;
using CHD.Client.FileOperation.Container;
using CHD.Client.Marker.Factory;
using CHD.Client.View;
using CHD.Client.ViewModel.Components;
using CHD.Graveyard.Token.Releaser;
using CHD.Settings.Mode;
using CHD.Wpf;

namespace CHD.Client.ViewModel
{
    internal class CurrentStatusViewModel : BaseViewModel
    {
        private readonly Settings.MainSettings _mainSettings;
        private readonly IEventMarkerFactory _eventMarkerFactory;
        private readonly IViewChanger _viewChanger;
        private readonly IActualFileOperationsContainer _actualFileOperationsContainer;
        private readonly IBackgroundReleaser _backgroundReleaser;

        public string WorkMode
        {
            get
            {
                return
                    ModeEnumHelper.GetDescription(_mainSettings.Mode);
            }
        }

        public string MarkerStatus
        {
            get
            {
                var result = string.Format(
                    "{0}{1}",
                    _eventMarkerFactory.IsMarkerCreated ? "Захвачен" : "Не захвачен",
                    _backgroundReleaser.IsReleaseInProgress ? " (отпускается)" : string.Empty
                    );

                return
                    result;
            }
        }

        public ObservableCollection2<ActualFileOperationWrapper> ActualFileActions
        {
            get;
            private set;
        }


        private ICommand _showMarkerHistoryCommand;
        public ICommand ShowMarkerHistoryCommand
        {
            get
            {
                if (_showMarkerHistoryCommand == null)
                {
                    _showMarkerHistoryCommand = new RelayCommand(
                        j =>
                        {
                            _viewChanger.ChangeForm(typeof(MarkerHistoryView));

                            OnPropertyChanged(string.Empty);
                        },
                        j => true);
                }

                return
                    _showMarkerHistoryCommand;
            }
        }

        public CurrentStatusViewModel(
            Dispatcher dispatcher,
            Settings.MainSettings mainSettings,
            IEventMarkerFactory eventMarkerFactory,
            IViewChanger viewChanger,
            IActualFileOperationsContainer actualFileOperationsContainer,
            IBackgroundReleaser backgroundReleaser
            )
            : base(dispatcher)
        {
            if (mainSettings == null)
            {
                throw new ArgumentNullException("mainSettings");
            }
            if (eventMarkerFactory == null)
            {
                throw new ArgumentNullException("eventMarkerFactory");
            }
            if (viewChanger == null)
            {
                throw new ArgumentNullException("viewChanger");
            }
            if (actualFileOperationsContainer == null)
            {
                throw new ArgumentNullException("actualFileOperationsContainer");
            }
            if (backgroundReleaser == null)
            {
                throw new ArgumentNullException("backgroundReleaser");
            }
            _mainSettings = mainSettings;
            _eventMarkerFactory = eventMarkerFactory;
            _viewChanger = viewChanger;
            _actualFileOperationsContainer = actualFileOperationsContainer;
            _backgroundReleaser = backgroundReleaser;

            ActualFileActions = new ObservableCollection2<ActualFileOperationWrapper>();

            _eventMarkerFactory.MarkerStatusChangedEvent += OnMarkerStatusChanged;
            _actualFileOperationsContainer.FileOperationChangedEvent += ActualFileOperationsContainerOnFileOperationChanged;
            _backgroundReleaser.BackgroundReleaserChangedEvent += BackgroundReleaserOnBackgroundReleaserChanged;
        }

        private void BackgroundReleaserOnBackgroundReleaserChanged()
        {
            OnPropertyChanged(string.Empty);
        }

        private void ActualFileOperationsContainerOnFileOperationChanged(
            bool created, 
            ActualFileOperationWrapper operationWrapper
            )
        {
            if (created)
            {
                BeginInvoke(
                    () => ActualFileActions.Add(operationWrapper)
                    );
            }
            else
            {
                BeginInvoke(
                    () => ActualFileActions.Remove(operationWrapper)
                    );
            }

            OnPropertyChanged(string.Empty);
        }

        private void OnMarkerStatusChanged(
            bool taken,
            Exception exception
            )
        {
            OnPropertyChanged(string.Empty);
        }
    }
}
