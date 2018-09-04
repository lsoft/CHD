using System;
using CHD.Tests.CompositionRoot.Modules.Fakes;
using CHD.Tests.CompositionRoot.Modules.Fakes.Stat;
using Ninject.Modules;
using PerformanceTelemetry;
using PerformanceTelemetry.Container;
using PerformanceTelemetry.Container.Saver;
using PerformanceTelemetry.Container.Saver.Item;
using PerformanceTelemetry.ErrorContext;
using PerformanceTelemetry.Payload;
using PerformanceTelemetry.Record;
using PerformanceTelemetry.ThreadIdProvider;
using PerformanceTelemetry.Timer;
using ProxyGenerator.Generator;
using ProxyGenerator.Payload;

namespace CHD.Tests.CompositionRoot.Modules
{
    public sealed class ProxyModule : NinjectModule
    {
        private readonly ITelemetryLogger _logger;

        public ProxyModule(
            ITelemetryLogger logger
            )
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _logger = logger;
        }

        public override void Load()
        {
            Bind<ITelemetryLogger>()
                .ToConstant(_logger)
                .InSingletonScope()
                ;

            Bind<IItemSaverFactory>()
                .To<StatSaverFactory>()
                .InSingletonScope()
                ;

            Bind<IPerformanceTimerFactory>()
                .To<PerformanceTimerFactory>()
                .InSingletonScope()
                ;

            Bind<IPerformanceRecordFactory>()
                .To<PerformanceRecordFactory>()
                .InSingletonScope()
                ;

            Bind<IErrorContextFactory>()
                .To<EmptyContextFactory>()
                .InSingletonScope()
                ;

            Bind<IThreadIdProvider>()
                .To<ThreadIdProvider>()
                .InSingletonScope()
                ;

            Bind<IPerformanceContainer>()
                .To<PerformanceContainer>()
                .InSingletonScope()
                ;

            Bind<IProxyPayloadFactory>()
                .To<PerformanceTelemetryPayloadFactory>()
                .InSingletonScope()
                ;

            Bind<IProxyTypeGenerator>()
                .To<ProxyTypeGenerator>()
                .InSingletonScope()
                ;

            Bind<IPerformanceSaver>()
                .To<EventBasedSaver>()
                .InSingletonScope()
                ;

            Bind<StatRecordContainer>()
                .ToSelf()
                .InSingletonScope()
                ;
        }
    }
}
