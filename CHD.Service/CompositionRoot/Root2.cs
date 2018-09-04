using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Threading;
using CHD.Common;
using CHD.Common.Breaker;
using CHD.Common.FileSystem;
using CHD.Common.Structure.Cleaner;
using CHD.Disk;
using CHD.Disk.Cleaner;
using CHD.Email.Settings;
using CHD.Ninject;
using CHD.Plugin.Infrastructure;
using CHD.Remote.FileSystem;
using CHD.Service.CompositionRoot.Fakes;
using CHD.Service.CompositionRoot.Modules;
using CHD.Service.Runner;
using CHD.Service.Wcf;
using CHD.Settings.Mode;
using Ninject;
using Ninject.Activation;
using Ninject.Parameters;

namespace CHD.Service.CompositionRoot
{
    internal sealed class Root2
    {
        internal const string LocalStructureCleanerKey = "LocalStructureCleaner";
        internal const string RemoteStructureCleanerKey = "RemoteStructureCleaner";

        private const string WindowsIsShuttingDownBreakSignal = "Windows is shutting down break signal";

        private readonly Arguments _arguments;

        private readonly StandardKernel _container;

        private Action<string> _logger;

        private IPluginBinder _pluginBinder;

        private bool _started = false;
        private bool _stopped = false;
        
        private long _disposed = 0L;

        public Root2(
            Arguments arguments
            )
        {
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }

            _arguments = arguments;

            _logger = (message) => { };

            _container = new StandardKernel();
        }

        public void BindAll(
            
            )
        {
            //логгер
            var loggerLocaleModule = new LoggerModule(
                64,
                _arguments.IsServiceMode
                );
            _container.Load(loggerLocaleModule);

            var logger = _container.Get<IDisorderLogger>();

            _logger = logger.LogMessage;

            //a guard from service hardcore murder
            RegistryPatcher.WaitToKillServiceTimeoutPatch(logger);


            var settingsModule = new SettingsModule(
                _arguments
                );
            _container.Load(settingsModule);

            var serviceSettings = _container.Get<ServiceSettings>();

            if (!Directory.Exists(serviceSettings.WatchFolder))
            {
                Directory.CreateDirectory(serviceSettings.WatchFolder);
            }

            var commonComponentsModule = new CommonComponentsModule(
                _arguments,
                serviceSettings
                );
            _container.Load(commonComponentsModule);

            var onlineModule = new OnlineModule();
            _container.Load(onlineModule);

            var fileSystemModule = new SyncModule(
                _arguments,
                serviceSettings
                );
            _container.Load(fileSystemModule);


            var proxyModule = new ProxyModule(
                new FakeTelemetryLogger()
                );
            _container.Load(proxyModule);


            var localModule = new DiskModule(
                );
            _container.Load(localModule);


            switch (serviceSettings.Mode)
            {
                case ModeEnum.Disk:
                {
                    throw new NotSupportedException("not yet");
                    break;
                }
                case ModeEnum.Email:
                {
                    var emailModule = new EmailModule(
                        serviceSettings.WatchFolderName
                        );
                    _container.Load(emailModule);
                    break;
                }
                case ModeEnum.CloudMailru:
                {
                    var mailRuModule = new MailRuModule(
                        serviceSettings.WatchFolderName
                        );
                    _container.Load(mailRuModule);
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(serviceSettings.Mode.ToString());
                }
            }

            var wcfModule = new WcfModule(
                );
            _container.Load(wcfModule);

            var pluginModule = new PluginModule(
                _arguments
                );
            _container.Load(pluginModule);

            ////телеметрия
            //var telemetryModule = new LocalFileProxyModule(
            //    telemetryOff,
            //    TimeSpan.Zero //этот параметр в конечном счете на клиенте не используется
            //    );
            //_container.Load(telemetryModule);

            ////основные объекты
            //var clientGuid = clientConfiguration.ReadValue<Guid>(ClientGuidKey, new GuidKeyValueConverter());
            //var supervisorManualMode = clientConfiguration.ReadValue<bool>(SupervisorManualModeKey);

            //var commonComponentsModule = new CommonComponentsModule(
            //    isServiceMode,
            //    clientGuid,
            //    supervisorManualMode
            //    );
            //_container.Load(commonComponentsModule);

            ////настройки репозиториев
            //var slavePackageRepositoryRoot = clientConfiguration.ReadValue<string>(SlavePackageRepositoryRootKey);
            //var slavePackageRepositoryGuid = clientConfiguration.ReadValue<Guid>(SlavePackageRepositoryGuidKey, new GuidKeyValueConverter());

            //var slaveRepositorParameters = new PackageRepositoryParameters(
            //    slaveConnectionString,
            //    slavePackageRepositoryRoot,
            //    slavePackageRepositoryGuid
            //    );

            //var selfupdatePackageRepositoryRoot = clientConfiguration.ReadValue<string>(SelfupdatePackageRepositoryRootKey);
            //var selfupdatePackageRepositoryGuid = clientConfiguration.ReadValue<Guid>(SelfupdatePackageRepositoryGuidKey, new GuidKeyValueConverter());

            //var selfupdateRepositorParameters = new PackageRepositoryParameters(
            //    selfupdateConnectionString,
            //    selfupdatePackageRepositoryRoot,
            //    selfupdatePackageRepositoryGuid
            //    );

            ////чистка репозиториев
            //try
            //{
            //    var fileSystemHelper = _container.Get<IFileSystemHelper>();
            //    var revertableActionContainerFactory = _container.Get<IRevertableActionContainerFactory>();
            //    var loggerSectionFactory = _container.Get<IDisorderLoggerSectionFactory>();

            //    var cleanupStrategy = new SafelyCleanupExecutor(
            //        this._container,
            //        fileSystemHelper,
            //        revertableActionContainerFactory,
            //        new DirectCleanupExecutor(
            //            new ThresholdManifestToDeleteProvider(
            //                -1),
            //            loggerSectionFactory
            //            ),
            //        (root, parameters, repositoryName) =>
            //        {
            //            var rb = new RepositoryBinder(
            //                root,
            //                SqlServerProxyModule.PerformanceProxyPayloadFactoryProvider,
            //                repositoryName
            //                );

            //            rb.BindRepositoryInfratructure(parameters);
            //        }
            //        );

            //    cleanupStrategy.DoCleanup(
            //        slaveRepositorParameters
            //        );
            //    cleanupStrategy.DoCleanup(
            //        selfupdateRepositorParameters
            //        );
            //}
            //catch (Exception excp)
            //{
            //    logger.LogException(excp);
            //}

            ////репозиторий раба

            //var slaveRepositoryModule = new SlaveRepositoryModule(
            //    slaveRepositorParameters
            //    );
            //_container.Load(slaveRepositoryModule);

            ////репозиторий самообновления

            //var selfupdateRepositoryModule = new SelfupdateRepositoryModule(
            //    selfupdateRepositorParameters
            //    );
            //_container.Load(selfupdateRepositoryModule);

            ////расширенная диагностика
            //var schedulerTimeoutMsec = clientConfiguration.ReadValue<int>(SchedulerTimeoutMsecKey);

            //var extendedDiagnosticModule = new ExtendedDiagnosticModule(schedulerTimeoutMsec);
            //_container.Load(extendedDiagnosticModule);

            ////клиентский канал
            //var clientChannelEndpointAddress = clientConfiguration.ReadValue<string>(ClientChannelEndpointAddressKey);
            //var securedChannel = clientConfiguration.ReadValue<bool>(SecuredChannelKey);
            //var authorityCertificateName = clientConfiguration.ReadValue<string>(AuthorityCertificateNameKey);
            //var dnsIdentity = clientConfiguration.ReadValue<string>(DnsIdentityKey);

            //var clientChannelModule = new ClientChannelModule(
            //    clientChannelEndpointAddress,
            //    securedChannel,
            //    authorityCertificateName,
            //    dnsIdentity
            //    );
            //_container.Load(clientChannelModule);

            ////Система мониторинга
            //var snmpEnabled = clientConfiguration.ReadValue<bool>(SNMPEnabledKey);

            //var speakerClientModule = new SpeakerClientModule(
            //    snmpEnabled
            //    );
            //_container.Load(speakerClientModule);

            //Func<List<IClientPluginState>> pluginsStateListFunc =
            //    () =>
            //    {
            //        var allPlugins = new List<ICHDPlugin>();

            //        if (this._pluginBinder != null && this._pluginBinder.Plugins != null)
            //        {
            //            allPlugins.AddRange(this._pluginBinder.Plugins);
            //        }

            //        return
            //            allPlugins.ConvertAll(j => (IClientPluginState) new ClientPluginAdapter(j.GetType().Assembly.GetName().Name, j.CurrentInformation));
            //    };

            ////ГУЙ клиента
            //var failEventThreshold = clientConfiguration.ReadValue<int>(FailEventThresholdKey);
            //var guiAdaptersModule = new GuiAdaptersModule(
            //    clientChannelEndpointAddress,
            //    failEventThreshold,
            //    pluginsStateListFunc
            //    );
            //_container.Load(guiAdaptersModule);

            ////команды
            //var commandSessionTimeoutMsec = clientConfiguration.ReadValue<int>(CommandSessionTimeoutMsecKey);
            //var commandSessionRootFolderMsec = clientConfiguration.ReadValue<string>(CommandSessionRootFolderMsecKey);

            //var commandSessionModule = new CommandSessionModule(
            //    commandSessionTimeoutMsec,
            //    commandSessionRootFolderMsec
            //    );
            //_container.Load(commandSessionModule);

            ////общие плагины
            //var commonModulesFolder = clientConfiguration.ReadValue<string>(CommonModulesFolderKey);

            _pluginBinder = _container.Get<IPluginBinder>();

            if (serviceSettings.IsPluginsExists)
            {
                _pluginBinder.InitPluginsFromFolder(
                    serviceSettings.PluginFolder
                    );
            }

            ////плагины клиентского канала
            //var clientChannelPluggedModulesFolder = clientConfiguration.ReadValue<string>(ClientChannelPluggedModulesFolderKey);
            //_pluginBinder.InitPluginsFromFolder(clientChannelPluggedModulesFolder);
        }

        public void Start()
        {
            if (_started)
            {
                //уже стартовали

                return;
            }

            //if local clear arguments exists then we should to perform cleaning local file system
            if (_arguments.LocalClear)
            {
                var cleaner = _container.Get<IDiskFileSystemCleaner>();
                cleaner.SafelyClear();
            }

            //if remote clear arguments exists then we should to perform cleaning remote file system
            if (_arguments.RemoteClear)
            {
                var cleaner = _container.Get<IRemoteFileSystemCleaner>();
                cleaner.SafelyClear();
            }

            var logger = _container.Get<IDisorderLogger>();

            try
            {
                logger.LogFormattedMessage(
                    "Версия {0}",
                    VersionHelper.GetIdAndVersion()
                    );

                try
                {
                    //запускаем плагины
                    _pluginBinder.StartPlugins();
                }
                catch (Exception excp)
                {
                    logger.LogException(
                        excp,
                        "-- ОШИБКА ПРИ СТАРТЕ --: запускаем плагины"
                        );
                }

                try
                {
                    //асинхронно запускаем шедулер сканирования папки
                    var scheduledScannerRunner = _container.Get<IScheduledScannerRunner>();
                    scheduledScannerRunner.AsyncStart();
                }
                catch (Exception excp)
                {
                    logger.LogException(
                        excp,
                        "-- ОШИБКА ПРИ СТАРТЕ --: асинхронно запускаем шедулер сканирования папки"
                        );
                }

                try
                {
                    var serviceSettings = _container.Get<ServiceSettings>();
                    var listener = _container.Get<IWcfListener>();
                    listener.StartListen(
                        new EndpointAddress(
                            serviceSettings.DataChannelEndpoint
                            )
                        );
                }
                catch (Exception excp)
                {
                    logger.LogException(
                        excp,
                        "-- ОШИБКА ПРИ СТАРТЕ --: асинхронно запускаем шедулер сканирования папки"
                        );
                }

            }
            catch (Exception excp)
            {
                logger.LogException(
                    excp,
                    "-- ОШИБКА ПРИ СТАРТЕ --"
                    );
            }
            finally
            {
                _started = true;
            }
        }

        public void Stop()
        {
            if (!_started)
            {
                //не стартовали

                return;
            }
            if (_stopped)
            {
                //уже стопнулись

                return;
            }

            var logger = _container.Get<IDisorderLogger>();

            try
            {
                logger.LogMessage("$$ синхронно останавливаем шедулер сканирования папки");

                //синхронно останавливаем шедулер сканирования папки
                try
                {
                    var scheduledScannerRunner = _container.Get<IScheduledScannerRunner>();
                    scheduledScannerRunner.SyncStop();
                }
                catch (Exception excp)
                {
                    logger.LogException(excp);
                }


                logger.LogMessage("$$ останавливаем плагины");

                //останавливаем плагины
                try
                {
                    _pluginBinder.StopPlugins();
                }
                catch (Exception excp)
                {
                    logger.LogException(excp);
                }

                logger.LogMessage("$$ высвобождаем плагины");

                //высвобождаем плагины
                try
                {
                    _pluginBinder.ReleasePlugins();
                }
                catch (Exception excp)
                {
                    logger.LogException(excp);
                }
            }
            finally
            {
                _stopped = true;
            }
        }

        public void SetWindowsIsShuttingDown()
        {
            var breaker = _container.Get<IBreaker>();
            breaker.FireBreak(WindowsIsShuttingDownBreakSignal);
        }

        public void Dispose()
        {
            try
            {
                if(Interlocked.Exchange(ref _disposed, 1L) == 0L)
                {
                    _container.Dispose();
                }
            }
            catch (Exception excp)
            {
                if (_logger != null)
                {
                    _logger(
                        String.Format(
                            "{0}:{1}\r\n{2}",
                            excp.GetType().Name,
                            excp.Message,
                            excp.StackTrace));
                }
            }
        }
    }
}