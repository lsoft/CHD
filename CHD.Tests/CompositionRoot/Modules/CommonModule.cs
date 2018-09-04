using System;
using CHD.Common;
using CHD.Common.Breaker;
using CHD.Common.Diff;
using CHD.Common.Diff.Applier;
using CHD.Common.Diff.Constructor;
//using CHD.Common.FolderStructureProvider;
//using CHD.Common.KeyValueContainer;
//using CHD.Common.KeyValueContainer.SyncedVersion;
using CHD.Common.Operation;
using CHD.Common.Operation.Applier;
using CHD.Common.Operation.Applier.Factory;
using CHD.Common.Operation.Visitor;
using CHD.Common.OperationLog;
using CHD.Common.PathComparer;
using CHD.Common.Serializer;
using CHD.Common.Serializer.BinaryFormatter;
using CHD.Common.Sync;
using CHD.Common.Sync.Factory;
using CHD.Tests.FileSystem;
using CHD.Tests.FileSystem.Surgeon.Factory;
//using CHD.Tests.FolderStructureProvider;
using CHD.Tests.Other;
using CHD.Tests.Sync;
using CHD.Token.Releaser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject.Modules;

namespace CHD.Tests.CompositionRoot.Modules
{
    public sealed class CommonModule : NinjectModule
    {
        private readonly DebugLogger _logger;

        public CommonModule(
            DebugLogger logger
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
            Bind<IDisorderLogger, DebugLogger>()
                .ToConstant(_logger)
                .InSingletonScope()
                ;

            Bind<IOperationDumper>()
                .To<LoggerOperationDumper>()
                .InSingletonScope()
                ;

            //Bind<IKeyValueContainer>()
            //    .To<InMemoryKeyValueContainer>()
            //    .WhenInjectedExactlyInto<SyncedVersionContainer>()
            //    //.InSingletonScope() not a singleton!!!
            //    ;

            //Bind<IFolderStructureProvider>()
            //    .To<TestFolderStructureProvider>()
            //    .InSingletonScope()
            //    ;

            //Bind<IFolderStructureProvider>()
            //    .To<DefaultFolderStructureProvider>()
            //    .InSingletonScope()
            //    ;

            Bind<ISerializer>()
                .To<BinaryFormatterSerializer>()
                .InSingletonScope()
                ;

            Bind<IPathComparerProvider>()
                .To<WindowsPathComparerProvider>()
                .InSingletonScope()
                ;

            Bind<IBackgroundReleaser>()
                .To<BackgroundReleaser>()
                .InSingletonScope()
                ;

            Bind<IDiffConstructor>()
                .To<DiffConstructor>()
                .InSingletonScope()
                ;


            Bind<IBreaker, IReadBreaker>()
                .To<Breaker<InternalTestFailureException>>()
                .InSingletonScope()
                ;

            Bind<IOperationLogGenerator>()
                .To<OperationLogGenerator>()
                .InSingletonScope()
                ;

            Bind<IDiffApplier>()
                .To<DiffApplier>()
                .InSingletonScope()
                ;

            Bind<IOperationApplierFactory>()
                .To<OperationApplierFactory>()
                .InSingletonScope()
                ;




            Bind<FileSystemNavigator>()
                .ToSelf()
                //not a singleton!
                ;

            Bind<FileSystemBuilder>()
                .ToSelf()
                //not a singleton!
                ;
        }

    }
}