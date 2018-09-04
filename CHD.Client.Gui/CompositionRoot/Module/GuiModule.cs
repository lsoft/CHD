using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHD.Client.Gui.CompositionRoot.Helper;
using CHD.Client.Gui.ViewModel.Main;
using Ninject.Modules;

namespace CHD.Client.Gui.CompositionRoot.Module
{
    public sealed class GuiModule : NinjectModule
    {
        //internal const string MainWindowFactoryName = "MainWindowFactory";
        //internal const string DetailsWindowFactoryName = "DetailsWindowFactory";
       

        public override void Load()
        {
            Bind<System.Windows.Threading.Dispatcher>()
                .ToConstant(App.Current.Dispatcher)
                .InSingletonScope()
                ;




            Bind<MainWindow>()
                .To<MainWindow>()
                .InSingletonScope()
                ;

            Bind<MainViewModel>()
                .To<MainViewModel>()
                .InSingletonScope()
                ;

            Bind<IMainWindowFactory>()
                .To<MainWindowFactory>()
                .InSingletonScope()
                //.Named(MainWindowFactoryName)
                ;






            Bind<DetailsWindow>()
                .To<DetailsWindow>()
                //.InSingletonScope()
                ;

            Bind<DetailsViewModel>()
                .To<DetailsViewModel>()
                //.InSingletonScope()
                ;

            Bind<IDetailsWindowFactory>()
                .To<DetailsWindowFactory>()
                //.WhenInjectedExactlyInto<MainViewModel>()
                .InSingletonScope()
                //.Named(DetailsWindowFactoryName)
                ;
        }
    }
}
