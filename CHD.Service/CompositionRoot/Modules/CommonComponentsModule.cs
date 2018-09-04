using System;
using CHD.Common.KeyValueContainer;
using CHD.Common.PathComparer;
using CHD.Common.Serializer;
using CHD.Common.Serializer.BinaryFormatter;
using Ninject.Modules;

namespace CHD.Service.CompositionRoot.Modules
{
    public sealed class CommonComponentsModule : NinjectModule
    {
        private readonly Arguments _arguments;
        private readonly ServiceSettings _serviceSettings;

        public CommonComponentsModule(
            Arguments arguments,
            ServiceSettings serviceSettings
            )
        {
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
            if (serviceSettings == null)
            {
                throw new ArgumentNullException("serviceSettings");
            }
            _arguments = arguments;
            _serviceSettings = serviceSettings;
        }

        public override void Load()
        {

            Bind<ISerializer>()
                .To<BinaryFormatterSerializer>()
                .InSingletonScope()
                ;

            Bind<IPathComparerProvider>()
                .To<WindowsPathComparerProvider>()
                .InSingletonScope()
                ;

            Bind<IKeyValueContainer>()
                .To<FileKeyValueContainer>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "folderPath",
                    _serviceSettings.PropertiesFolder
                )
                ;

        }

    }
}