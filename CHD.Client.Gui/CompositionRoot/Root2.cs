using System;
using System.Threading;
using CHD.Client.Gui.CompositionRoot.Helper;
using CHD.Client.Gui.CompositionRoot.Module;
using CHD.Client.Gui.DataFlow;
using CHD.Client.Gui.DataFlow.Retriever;
using CHD.Common;
using Ninject;

namespace CHD.Client.Gui.CompositionRoot
{
    internal sealed class Root2
    {
        internal const string LocalStructureCleanerKey = "LocalStructureCleaner";
        internal const string RemoteStructureCleanerKey = "RemoteStructureCleaner";

        private readonly StandardKernel _container;

        private Arguments _arguments;

        private Action<string> _logger;

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
                64
                );
            _container.Load(loggerLocaleModule);

            var logger = _container.Get<IDisorderLogger>();

            _logger = logger.LogMessage;

            var settingsModule = new SettingsModule(
                _arguments
                );
            _container.Load(settingsModule);

            var commonComponentsModule = new CommonComponentsModule(
                );

            _container.Load(commonComponentsModule);

            var guiModule = new GuiModule(
                );

            _container.Load(guiModule);

            var wcfModule = new WcfModule(
                );

            _container.Load(wcfModule);


        }

        public void Start()
        {
            if (_started)
            {
                //уже стартовали

                return;
            }

            var logger = _container.Get<IDisorderLogger>();

            try
            {
                try
                {
                    var dataRetriever = _container.Get<IDataRetriever>();
                    dataRetriever.AsyncStart();
                }
                catch (Exception excp)
                {
                    logger.LogException(
                        excp,
                        "-- ОШИБКА ПРИ СТАРТЕ --: асинхронно запускаем дата ретривер"
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

        //public IWindowFactory GetMainWindowFactory()
        //{
        //    return
        //        _container.Get<IWindowFactory>(GuiModule.MainWindowFactoryName);
        //}

        //public IWindowFactory GetDetailsWindowFactory()
        //{
        //    return
        //        _container.Get<IWindowFactory>(GuiModule.DetailsWindowFactoryName);
        //}

        public T Get<T>()
        {
            return
                _container.Get<T>();
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
                logger.LogMessage("$$ синхронно останавливаем дата ретривер");

                try
                {
                    var dataRetriever = _container.Get<IDataRetriever>();
                    dataRetriever.SyncStop();
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