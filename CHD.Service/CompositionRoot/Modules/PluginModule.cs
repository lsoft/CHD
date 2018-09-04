using System;
using CHD.Plugin.Infrastructure;
using Ninject.Modules;

namespace CHD.Service.CompositionRoot.Modules
{
    public sealed class PluginModule : NinjectModule
    {
        private readonly Arguments _arguments;

        public PluginModule(
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
            Bind<IPluginLoader>()
                .To<PluginLoader>()
                .InSingletonScope()
                ;

            Bind<IPluginBinder>()
                .To<PluginBinder>()
                .InSingletonScope()
                ;
        }

    }
}