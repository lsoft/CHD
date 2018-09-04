using System;
using CHD.Settings.Controller;
using Ninject.Modules;

namespace CHD.Client.Gui.CompositionRoot.Module
{
    public sealed class SettingsModule : NinjectModule
    {
        private readonly Arguments _arguments;

        public SettingsModule(
            Arguments arguments
            )
        {
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
            _arguments = arguments;
        }

        public override void Load()
        {
            Bind<ISettings>()
                .To<Settings.Controller.Settings>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "filePath",
                    _arguments.SettingsFile
                )
                ;

            Bind<AppSettings>()
                .ToSelf()
                .InSingletonScope()
                ;
        }

    }
}