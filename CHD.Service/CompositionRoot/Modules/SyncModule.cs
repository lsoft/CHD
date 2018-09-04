using System;
using CHD.Common.Breaker;
using CHD.Common.Diff;
using CHD.Common.Diff.Applier;
using CHD.Common.Diff.Constructor;
using CHD.Common.FileSystem;
//using CHD.Common.FolderStructureProvider;
//using CHD.Common.KeyValueContainer;
//using CHD.Common.KeyValueContainer.SyncedVersion;
using CHD.Common.Operation.Applier;
using CHD.Common.Operation.Applier.Factory;
using CHD.Common.Operation.Visitor;
using CHD.Common.OperationLog;
using CHD.Common.Others;
using CHD.Common.PathComparer;
using CHD.Common.Scanner;
using CHD.Common.Serializer;
using CHD.Common.Sync;
using CHD.Common.Sync.Factory;
using CHD.Common.Sync.Online;
using CHD.Common.Sync.Provider;
using CHD.Common.Sync.Report.Journal;
using CHD.Common.Sync.Storeable;
using CHD.Common.Watcher;
using CHD.Disk.FileSystem;
using CHD.Remote.FileSystem;
using CHD.Service.Runner;
using CHD.Token.Container;
using CHD.Token.Releaser;
using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using Ninject.Parameters;

namespace CHD.Service.CompositionRoot.Modules
{
    public sealed class SyncModule : NinjectModule
    {
        private readonly Arguments _arguments;
        private readonly ServiceSettings _serviceSettings;

        public SyncModule(
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
            //Bind<ISyncedVersionProvider, ISyncedVersionContainer>()
            //    .To<SyncedVersionContainer>()
            //    .InSingletonScope()
            //    ;

            Bind<IOperationApplierFactory>()
                .To<OperationApplierFactory>()
                .WhenInjectedExactlyInto<OnlineOperationApplierFactory>()
                .InSingletonScope()
                ;

            Bind<IOperationApplierFactory>()
                .To<OnlineOperationApplierFactory>()
                .InSingletonScope()
                ;

            Bind<ISyncJournal>()
                .To<DiskSyncJournal>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "filePath",
                    _serviceSettings.SyncJournalFile
                    )
                .WithConstructorArgument(
                    "storeDays",
                    _serviceSettings.SyncReportStoreDays
                    )
                ;

            Bind<IScheduledScannerRunner>()
                .To<ScheduledScannerRunner>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "timeoutMsec",
                    _serviceSettings.ScheduledScanTimeoutMsec
                )
                ;

            Bind<IBackgroundReleaser>()
                .To<BackgroundReleaser>()
                .InSingletonScope()
                ;

            Bind<IBreaker, IReadBreaker>()
                .To<Breaker<CHDException>>()
                .InSingletonScope()
                ;

            Bind<IOperationLogGenerator>()
                .To<OperationLogGenerator>()
                .InSingletonScope()
                ;

            Bind<IDiffConstructor>()
                .To<DiffConstructor>()
                .WhenInjectedExactlyInto<OnlineDiffConstructor>()
                .InSingletonScope()
                ;

            Bind<IDiffConstructor>()
                .To<OnlineDiffConstructor>()
                .InSingletonScope()
                ;

            Bind<IDiffApplier>()
                .To<DiffApplier>()
                .WhenInjectedExactlyInto<OnlineDiffApplier>()
                .InSingletonScope()
                ;

            Bind<IDiffApplier>()
                .To<OnlineDiffApplier>()
                .InSingletonScope()
                ;

            Bind<ISynchronizerFactory>()
                .To<SynchronizerFactory>()
                .WhenInjectedExactlyInto<StoreableSynchronizerFactory>()
                .InSingletonScope()
                ;

            Bind<ISynchronizerFactory>()
                .To<StoreableSynchronizerFactory>()
                .WhenInjectedExactlyInto<OnlineSynchronizerFactory>()
                .InSingletonScope()
                ;

            Bind<ISynchronizerFactory>()
                .To<OnlineSynchronizerFactory>()
                .InSingletonScope()
                ;


            Bind<ISynchronizerProvider>()
                .To<SynchronizerProvider>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "localConnector",
                    c => DiskModule.GetLocalConnector(c)
                    )
                .WithConstructorArgument(
                    "remoteConnector",
                    c => GetRemoteConnector(c)
                    )
                ;

            Bind<IScanner>()
                .To<DefaultScanner>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "rootFolderPath",
                    _serviceSettings.WatchFolder
                    )
                .WithConstructorArgument(
                    "skipFolders",
                    _serviceSettings.SkipFolders
                    )
                ;

            Bind<ITokenContainer, ITokenProvider>()
                .To<KeyValueTokenContainer>()
                .InSingletonScope()
                ;

            Bind<IFileSystemWatcher>()
                .To<FileSystemWatcher>()
                .WhenInjectedExactlyInto<FileWatcherController>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "targetPath",
                    _serviceSettings.WatchFolder
                    )
                ;

            Bind<IFileWatcherController>()
                .To<FileWatcherController>()
                .InSingletonScope()
                ;
        }

        private static IFileSystemConnector GetRemoteConnector(
            IContext context
            )
        {
            var result = context.Kernel.Get<RemoteFileSystemConnector>(
                new IParameter[]
                {
                    new Parameter(
                        Root2.RemoteStructureCleanerKey,
                        (object) null,
                        true
                        ),
                }
                );

            return
                result;
        }

    }
}