using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CHD.Common.Crypto;
using CHD.Installer.CompositionRoot.Components;
using CHD.Installer.Scanner;
using CHD.Installer.View;
using CHD.Installer.ViewModel;
using CHD.Installer.ViewModel.Edit;
using CHD.Wpf;
using Ninject;
using Ninject.Modules;

namespace CHD.Installer.CompositionRoot
{
    internal sealed class UserInterfaceModule : NinjectModule
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


            Bind<MainViewModel>()
                .ToSelf()
                .WithConstructorArgument(
                    "mainSettingsScanner",
                    c => c.Kernel.Get<SettingsFileScanner>(Root.MainKey)
                    )
                .WithConstructorArgument(
                    "emailSettingsScanner",
                    c => c.Kernel.Get<SettingsFileScanner>(Root.EmailKey)
                    )
                .WithConstructorArgument(
                    "mailRuCloudSettingsScanner",
                    c => c.Kernel.Get<SettingsFileScanner>(Root.MailRuKey)
                    )
                .WithConstructorArgument(
                    "realCrypto",
                    c => c.Kernel.Get<ICrypto>(SettingsModule.RealCryptoKey)
                    )
                .WithConstructorArgument(
                    "fakeCrypto",
                    c => c.Kernel.Get<ICrypto>(SettingsModule.FakeCryptoKey)
                    )
                ;

            Bind<View.MainWindow>()
                .ToSelf()
                .InSingletonScope()
                ;



            Bind<EditViewModel>()
                .ToSelf()
                ;


            Bind<EditWindow>()
                .ToSelf()
                ;

            Bind<IEditWindowFactory>()
                .To<EditWindowFactory>()
                ;

        }
    }
}
