using System;
using System.ComponentModel;
using System.IO;
using CHD.Client.CompositionRoot.WpfFactory;
using CHD.Client.Crypto;
using CHD.Client.FileOperation.ExecutionUnit;
using CHD.Client.FileOperation.Pusher;
using CHD.Client.Marker.Factory;
using CHD.Client.ViewModel;
using CHD.Client.ViewModel.Components;
using CHD.Common;
using CHD.Common.Crypto;
using CHD.Common.HashGenerator;
using CHD.Common.KeyValueContainer;
using CHD.Common.KeyValueContainer.Order;
using CHD.Dynamic.Scheduler;
using CHD.Dynamic.Scheduler.Logger;
using CHD.Dynamic.Scheduler.SchedulerThread.Factory;
using CHD.Dynamic.Scheduler.Task;
using CHD.Dynamic.Scheduler.WaitGroup;
using CHD.Dynamic.Scheduler.WaitGroup.Standard;
using CHD.Email.Operation;
using CHD.Email.ServiceCode;
using CHD.Email.Token;
using CHD.FileSystem.FileWrapper;
using CHD.FileSystem.Watcher;
using CHD.Graveyard.ExclusiveAccess.Factory;
using CHD.Graveyard.Graveyard;
using CHD.Graveyard.Marker;
using CHD.Graveyard.Operation;
using CHD.Graveyard.Token.Factory;
using CHD.Graveyard.Token.Releaser;
using CHD.Local.Operation;
using CHD.Local.Token;
using CHD.MailRuCloud.Operation;
using CHD.MailRuCloud.ServiceCode;
using CHD.MailRuCloud.Token;
using CHD.Pull;
using CHD.Pull.Components.Factory;
using CHD.Push;
using CHD.Push.ActivityPool;
using CHD.Push.FileChangeWatcher;
using CHD.Push.Proxy;
using CHD.Push.Pusher.Factory;
using CHD.Push.Task;
using CHD.Push.Task.Factory;
using CHD.Push.Task.GuidProvider;
using CHD.Push.Task.SaverLoader;
using CHD.Push.Task.Store;
using CHD.Settings;
using CHD.Settings.Controller;
using CHD.Settings.Mode;
using Ninject;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using Ninject.Syntax;
using FileSystemWatcher = CHD.FileSystem.Watcher.FileSystemWatcher;

namespace CHD.Client.CompositionRoot.Module
{
    internal class MainModule : NinjectModule
    {
        internal const string RealCryptoKey = "RealCrypto";
        internal const string FakeCryptoKey = "FakeCrypto";

        private readonly string _settingsFilePath;

        public MainModule(
            string settingsFilePath
            )
        {
            if (settingsFilePath == null)
            {
                throw new ArgumentNullException("settingsFilePath");
            }

            _settingsFilePath = settingsFilePath;
        }

        public override void Load()
        {
            Bind<ISettings>()
                .To<Settings.Controller.Settings>()
                .WhenInjectedExactlyInto<MainSettings>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "filePath",
                    _settingsFilePath
                    )
                .WithConstructorArgument(
                    "crypto",
                    c => c.Kernel.Get<ICrypto>(FakeCryptoKey)
                    )
                ;

            Bind<MainSettings>()
                .ToSelf()
                .InSingletonScope()
                ;





            Bind<IFileWrapper>()
                .To<RealFile>()
                ;

            Bind<IFileWrapperFactory>()
                .To<FileWrapperFactory>()
                .InSingletonScope()
                ;




            Bind<IKeyValueContainer>()
                .To<FileKeyValueContainer>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "folderPath",
                    c => c.Kernel.Get<Settings.MainSettings>().KeyValueFolder
                    )
                ;

            Bind<IOrderContainer>()
                .To<OrderContainer>()
                .InSingletonScope()
                ;





            Bind<IFileSystemWatcher>()
                .To<FileSystemWatcher>()
                .WhenInjectedExactlyInto<ExcludeFileSystemWatcher>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "targetPath",
                    c => c.Kernel.Get<Settings.MainSettings>().WatchFolder
                    )
                ;

            Bind<IFileSystemWatcher, IExcluder>()
                .To<ExcludeFileSystemWatcher>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "targetPath",
                    c => c.Kernel.Get<Settings.MainSettings>().WatchFolder
                    )
                ;

            Bind<IFileChangeWatcher>()
                .To<DefaultFileChangeWatcher>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "watchFolderPath",
                    c => c.Kernel.Get<Settings.MainSettings>().WatchFolder
                    )
                ;




            Bind<ITimeoutActivityPool, IActivityPool>()
                .To<TimeoutActivityPool>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "liveTime",
                    c => c.Kernel.Get<Settings.MainSettings>().LiveTime
                    )
                .WithConstructorArgument(
                    "delayTime",
                    c => c.Kernel.Get<Settings.MainSettings>().DelayTime
                    )
                ;



            Bind<IAlgorithmFactory>()
                .To<AlgorithmFactory>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "maxFileBlockSize",
                    c => c.Kernel.Get<GraveyardSettings>().MaxFileBlockSize
                    )
                .WithConstructorArgument(
                    "minFileBlockSize",
                    c => c.Kernel.Get<GraveyardSettings>().MinFileBlockSize
                    )
                ;


            Bind<IHashGenerator>()
                .To<MD5HashGenerator>()
                .InSingletonScope()
                ;


            Bind<IPool2SchedulerProxy>()
                .To<Pool2SchedulerProxy>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "pushTimeoutAfterFailureMsec",
                    c => c.Kernel.Get<Settings.MainSettings>().PushTimeoutAfterFailureMsec
                    )
                ;

            Bind<IAlgorithmGuidProvider>()
                .To<HashAlgorithmGuidProvider>()
                .InSingletonScope()
                ;




            Bind<IPusherFactory>()
                .To<PusherFactory>()
                .WhenInjectedExactlyInto<FileOperationPusherFactory>()
                .InSingletonScope()
                ;

            Bind<IPusherFactory>()
                .To<FileOperationPusherFactory>()
                .InSingletonScope()
                ;

            Bind<IAlgorithmPermanentStore>()
                .To<AlgorithmPermanentStore>()
                .WhenInjectedExactlyInto<FreshAlgorithmPermanentStore>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "filePath", 
                    c => c.Kernel.Get<Settings.MainSettings>().PermanentStoreFile
                    )
                ;

            Bind<IAlgorithmPermanentStore>()
                .To<FreshAlgorithmPermanentStore>()
                .InSingletonScope()
                ;



            Bind<IOperationContainerFactory>()
                .ToFactory()
                .InSingletonScope()
                ;

            Bind<IOperationContainer>()
                .To<OperationContainer>()
                //not a singleton
                ;

            Bind<IGraveyard>()
                .To<Graveyard.Graveyard.Graveyard>()
                .InSingletonScope()
                ;

            Bind<IExclusiveAccessFactory>()
                .To<ExclusiveAccessFactory>()
                .InSingletonScope()
                ;

            Bind<IMarkerFactory>()
                .To<MarkerFactory>()
                .WhenInjectedExactlyInto<EventMarkerFactory>()
                .InSingletonScope()
                ;

            Bind<IMarkerFactory, IEventMarkerFactory>()
                .To<EventMarkerFactory>()
                .InSingletonScope()
                ;

            Bind<IBackgroundReleaser>()
                .To<BackgroundReleaser>()
                .InSingletonScope()
                ;




            Bind<IExecutionUnitFactory>()
                .To<ExecutionUnitFactory>()
                .WhenInjectedExactlyInto<FileOperationExecutionUnitFactory>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "targetFolder",
                    c => c.Kernel.Get<Settings.MainSettings>().WatchFolder
                    )
                ;

            Bind<IExecutionUnitFactory>()
                .To<FileOperationExecutionUnitFactory>()
                .InSingletonScope()
                ;

            Bind<PullSchedulerTask>()
                .To<PullSchedulerTask>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "pullTimeoutAfterSuccessMsec",
                    c => c.Kernel.Get<Settings.MainSettings>().PullTimeoutAfterSuccessTimeoutMsec
                    )
                .WithConstructorArgument(
                    "pullTimeoutAfterFailureMsec",
                    c => c.Kernel.Get<Settings.MainSettings>().PullTimeoutAfterFailureTimeoutMsec
                    )
                ;



            Bind<IWaitGroupFactory>()
                .To<StandardWaitGroupFactory>()
                .InSingletonScope()
                ;


            Bind<IThreadFactory>()
                .To<StandardThreadFactory>()
                .InSingletonScope()
                ;

            Bind<ISchedulerLogger>()
                .To<SchedulerLoggerProxy>()
                .InSingletonScope()
                ;

            Bind<IScheduler>()
                .To<Scheduler>()
                .WhenInjectedExactlyInto<SaverLoaderScheduler>()
                .InSingletonScope()
                ;

            Bind<IScheduler>()
                .To<SaverLoaderScheduler>()
                .InSingletonScope()
                ;





            Bind<ICrypto>()
                .To<Gost28147>()
                .Named(RealCryptoKey)
                ;

            Bind<ICrypto>()
                .To<FakeCrypto>()
                .Named(FakeCryptoKey)
                ;




            Bind<ICryptoKeyContainer, ICryptoKeyController>()
                .To<CryptoKeyController>()
                .InSingletonScope()
                ;



            Bind<SeedWindow>()
                .ToSelf()
                .InSingletonScope()
                ;

            Bind<SeedViewModel>()
                .ToSelf()
                .InSingletonScope()
                ;

            Bind<SeedWindowFactory>()
                .ToSelf()
                .InSingletonScope()
                ;


            var settings = this.Kernel.Get<MainSettings>();


            //if (settings.IsSettingsEncoded)
            //{
            //    var swf = this.Kernel.Get<SeedWindowFactory>();
            //    var sw = swf.Create();
            //    sw.ShowDialog();
            //}

            var watchFolder = settings.WatchFolder;

            if (!Directory.Exists(watchFolder))
            {
                Directory.CreateDirectory(watchFolder);
            }

            switch (settings.Mode)
            {
                case ModeEnum.Local:
                    BindLocal();
                    break;
                case ModeEnum.Email:
                    BindEmail();
                    break;
                case ModeEnum.CloudMailru:
                    BindCloudMailRu();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Bind<ITokenFactory, ITokenController>()
                .To<MarkerAndTokenController>()
                .InSingletonScope()
                ;
        }

        private void BindLocal()
        {
            const string PseudoEmailBoxFolderName = "_PseudoEmailBox";

            //if (Directory.Exists(PseudoEmailBoxFolderName))
            //{
            //    Directory.Delete(PseudoEmailBoxFolderName, true);
            //}
            //Directory.CreateDirectory(PseudoEmailBoxFolderName);


            Bind<IOperationFactory>()
                .To<LocalOperationFactory>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "folderPath",
                    PseudoEmailBoxFolderName
                    )
                ;


            Bind<ITokenFactory, ITokenController>()
                .To<LocalTokenController>()
                .WhenInjectedExactlyInto<MarkerAndTokenController>()
                .InSingletonScope()
                ;

        }

        private void BindEmail()
        {
            Bind<ISettings>()
                .To<Settings.Controller.Settings>()
                .WhenInjectedExactlyInto<EmailSettings>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "filePath",
                    c => c.Kernel.Get<MainSettings>().GraveyardSettingsFile
                    )
                .WithConstructorArgument(
                    "crypto",
                    c => GetCrypto(c.Kernel)
                    )
                ;

            Bind<GraveyardSettings, EmailSettings>()
                .To<EmailSettings>()
                ;

            Bind<IOperationFactory>()
                .To<EmailOperationFactory>()
                .InSingletonScope()
                ;


            Bind<ITokenFactory, ITokenController>()
                .To<EmailTokenController>()
                .WhenInjectedExactlyInto<MarkerAndTokenController>()
                .InSingletonScope()
                ;
        }

        private void BindCloudMailRu()
        {
            Bind<ISettings>()
                .To<Settings.Controller.Settings>()
                .WhenInjectedExactlyInto<MailRuSettings>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "filePath",
                    c => c.Kernel.Get<MainSettings>().GraveyardSettingsFile
                    )
                .WithConstructorArgument(
                    "crypto",
                    c => GetCrypto(c.Kernel)
                    )
                ;

            Bind<GraveyardSettings, MailRuSettings>()
                .To<MailRuSettings>()
                ;

            Bind<IOperationFactory>()
                .To<MailRuOperationFactory>()
                .InSingletonScope()
                ;


            Bind<ITokenFactory, ITokenController>()
                .To<MailRuTokenController>()
                .WhenInjectedExactlyInto<MarkerAndTokenController>()
                .InSingletonScope()
                ;
        }

        private ICrypto GetCrypto(
            IResolutionRoot root
            )
        {
            var fakeCrypto = root.Get<ICrypto>(FakeCryptoKey);

            var ms = root.TryGet<MainSettings>();

            if (ms == null)
            {
                return
                    fakeCrypto;
            }

            var cryptoEnabled = ms.IsSettingsEncoded;

            if (!cryptoEnabled)
            {
                return
                    fakeCrypto;
            }

            var realCrypto = root.Get<ICrypto>(RealCryptoKey);


            var keyContainer = root.TryGet<ICryptoKeyContainer>();
            if (keyContainer != null)
            {
                var key = keyContainer.CryptoKey;
                if (key != null)
                {
                    realCrypto.LoadKey(key);
                }
            }

            return
                realCrypto;
        }
    }
}