using CHD.Common.Crypto;
using CHD.Installer.Scanner;
using CHD.Installer.Scanner.Euristic;
using CHD.Settings;
using CHD.Settings.Controller;
using Ninject;
using Ninject.Modules;
using Ninject.Planning.Bindings;

namespace CHD.Installer.CompositionRoot
{
    internal sealed class SettingsModule : NinjectModule
    {
        internal const string RealCryptoKey = "RealCrypto";
        internal const string FakeCryptoKey = "FakeCrypto";

        public override void Load()
        {

            Bind<SettingsFileScanner>()
                .ToSelf()
                .InSingletonScope()
                .Named(Root.MailRuKey)
                .WithConstructorArgument(
                    "folderPath",
                    "."
                    )
                .WithConstructorArgument(
                    "euristic",
                    c => c.Kernel.Get<ISettingsEuristic>(Root.MailRuKey)
                    )
                ;

            Bind<ISettingsEuristic>()
                .To<MailRuCloudSettingsEuristic>()
                .InSingletonScope()
                .Named(Root.MailRuKey)
                ;



            Bind<SettingsFileScanner>()
                .ToSelf()
                .InSingletonScope()
                .Named(Root.EmailKey)
                .WithConstructorArgument(
                    "folderPath",
                    "."
                    )
                .WithConstructorArgument(
                    "euristic",
                    c => c.Kernel.Get<ISettingsEuristic>(Root.EmailKey)
                    )
                ;

            Bind<ISettingsEuristic>()
                .To<EmailSettingsEuristic>()
                .InSingletonScope()
                .Named(Root.EmailKey)
                ;



            Bind<SettingsFileScanner>()
                .ToSelf()
                .InSingletonScope()
                .Named(Root.MainKey)
                .WithConstructorArgument(
                    "folderPath",
                    "."
                    )
                .WithConstructorArgument(
                    "euristic",
                    c => c.Kernel.Get<ISettingsEuristic>(Root.MainKey)
                    )
                ;

            Bind<ISettingsEuristic>()
                .To<MainSettingsEuristic>()
                .InSingletonScope()
                .Named(Root.MainKey)
                ;




            Bind<ICrypto>()
                .To<Gost28147>()
                .Named(RealCryptoKey)
                ;

            Bind<ICrypto>()
                .To<FakeCrypto>()
                .Named(FakeCryptoKey)
                ;




            Bind<ISettingsFactory>()
                .To<SettingsFactory>()
                .InSingletonScope()
                ;

            //Bind<IEncodedSettingsControllerFactory>()
            //    .To<EncodedSettingsControllerFactory>()
            //    .InSingletonScope()
            //    .WithConstructorArgument(
            //        "realCrypto",
            //        c => c.Kernel.Get<ICrypto>(RealCryptoKey)
            //        )
            //    ;



            //Bind<ISettingsController>()
            //    .To<SettingsController>()
            //    .InSingletonScope()
            //    .Named(RealCryptoKey)
            //    .WithConstructorArgument(
            //        "crypto",
            //        c => c.Kernel.Get<ICrypto>(RealCryptoKey)
            //        )
            //    ;

            //Bind<ISettingsController>()
            //    .To<SettingsController>()
            //    .InSingletonScope()
            //    .Named(FakeCryptoKey)
            //    .WithConstructorArgument(
            //        "crypto",
            //        c => c.Kernel.Get<ICrypto>(FakeCryptoKey)
            //        )
            //    ;

            //Bind<ISmartSettingsController>()
            //    .To<SmartSettingsController>()
            //    //not a singleton
            //    .WithConstructorArgument(
            //        "settingsController",
            //        c => c.Kernel.Get<ISettingsController>(FakeCryptoKey)
            //        )
            //    .WithConstructorArgument(
            //        "encodedSettingsController",
            //        c => c.Kernel.Get<IEncodedSettingsController>(RealCryptoKey)
            //        )
            //    ;


            Bind<KeyProvider>()
                .To<KeyProvider>()
                .InSingletonScope()
                ;
        }
    }
}