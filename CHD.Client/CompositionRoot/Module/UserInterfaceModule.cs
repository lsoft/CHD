using System.Windows;
using System.Windows.Threading;
using CHD.Client.CompositionRoot.WpfFactory;
using CHD.Client.View;
using CHD.Client.ViewModel;
using Ninject.Modules;

namespace CHD.Client.CompositionRoot.Module
{
    internal class UserInterfaceModule : NinjectModule
    {
        public override void Load()
        {
            Bind<Application>()
                .ToConstant(Application.Current)
                .InSingletonScope()
                ;

            Bind<Dispatcher>()
                .ToConstant(App.Current.Dispatcher)
                .InSingletonScope()
                ;




            Bind<CurrentStatusViewModel>()
                .To<CurrentStatusViewModel>()
                .InSingletonScope()
                ;

            Bind<CurrentStatusView>()
                .To<CurrentStatusView>()
                .InSingletonScope()
                ;

            Bind<IViewFactory>()
                .To<CurrentStatusViewFactory>()
                .InSingletonScope()
                ;





            Bind<MarkerHistoryViewModel>()
                .To<MarkerHistoryViewModel>()
                .InSingletonScope()
                ;

            Bind<MarkerHistoryView>()
                .To<MarkerHistoryView>()
                .InSingletonScope()
                ;

            Bind<IViewFactory>()
                .To<MarkerHistoryViewFactory>()
                .InSingletonScope()
                ;





            Bind<IViewChanger, MainViewModel>()
                .To<MainViewModel>()
                .InSingletonScope()
                ;

            Bind<MainWindow>()
                .To<MainWindow>()
                .InSingletonScope()
                ;


        }
    }
}
