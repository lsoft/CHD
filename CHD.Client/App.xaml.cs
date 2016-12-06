using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using CHD.Client.CompositionRoot;
using CHD.Client.CompositionRoot.WpfFactory;
using CHD.Client.Marker.History;
using CHD.Client.ViewModel;
using CHD.Common.Logger;
using CHD.Dynamic.Scheduler;
using CHD.Graveyard.Marker;
using CHD.Graveyard.Token.Factory;
using CHD.Graveyard.Token.Releaser;
using CHD.Pull;
using CHD.Push.ActivityPool;
using CHD.Push.FileChangeWatcher;
using CHD.Push.Task;
using CHD.Settings;

namespace CHD.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string SettingsArg = "-settings:";
        private const string DefaultSettingsFileNameArg = "settings.cfg";

        private readonly Root _root;

        public App(
            )
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            _root = new Root();
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var settingsFilePath = DefaultSettingsFileNameArg;
            var ss = e.Args.FirstOrDefault(j => j.StartsWith(SettingsArg, StringComparison.InvariantCultureIgnoreCase));
            if (ss != null)
            {
                settingsFilePath = ss.Substring(SettingsArg.Length);
            }

            //Инициализируем всё
            _root.Init(
                settingsFilePath
                );

            //подписываемся на контейнер захвата маркера

            //готовим основное окно
            var mainWindow = _root.Get<MainWindow>();
            App.Current.MainWindow = mainWindow;

            var mvm = _root.Get<MainViewModel>();
            mainWindow.DataContext = mvm;

            var viewFactories = _root.GetAll<IViewFactory>();
            mvm.InsertForms(viewFactories);

            //спрашиваем пароль к настройкам, если они зашифрованы
            var settings = _root.Get<MainSettings>();
            if (settings.IsSettingsEncoded)
            {
                var swf = _root.Get<SeedWindowFactory>();
                var sw = swf.Create();
                sw.ShowDialog();
            }

            //читаем контейнер захватов маркера
            var rc = _root.Get<IRecordContainer>();
            rc.Prepare();

            //проверяем, был ли отпущен токен при предыдущем завершении
            var markerFactory = _root.Get<IMarkerFactory>();

            if (markerFactory.IsMarkerCreated)
            {
                var tokenController = _root.Get<ITokenController>();
                var releaser = _root.Get<IBackgroundReleaser>();

                releaser.TryToReleaseAtBackgroundThread(
                    tokenController.TryToReleaseToken
                    );
            }

            ////запускаем контроллер пушеров
            ////в этот момент он загружает отложенные пушы
            //var pusherController = _root.Get<IPushController>();
            //pusherController.AsyncStart();

            //запускаем таймаут пул
            var timeoutPool = _root.Get<ITimeoutActivityPool>();
            timeoutPool.AsyncStart();

            //запускаем вотчер
            var watcher = _root.Get<IFileChangeWatcher>();
            watcher.AsyncStart();

            //запускаем пулл шедулер
            var scheduler = _root.Get<IScheduler>();
            scheduler.Start();

            //создаем задачу на провеку изменений в graveyard
            var pullTask = _root.Get<PullSchedulerTask>();
            scheduler.AddTask(pullTask);

            //показываем окно
            mainWindow.Show();
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            var logger = _root.Get<IDisorderLogger>();

            //останавливаем вотчер
            try
            {
                var watcher = _root.Get<IFileChangeWatcher>();
                watcher.Stop();
            }
            catch (Exception excp)
            {
                logger.LogException(excp);
            }

            //останавливаем шедулер
            try
            {
                var pullScheduler = _root.Get<IScheduler>();
                pullScheduler.Stop();
            }
            catch (Exception excp)
            {
                logger.LogException(excp);
            }
            //останавливаем пул таймаута
            try
            {
                var timeoutPool = _root.Get<ITimeoutActivityPool>();
                timeoutPool.SyncStop();
            }
            catch (Exception excp)
            {
                logger.LogException(excp);
            }

            ////останавливаем контроллер пушеров
            //try
            //{
            //    var pusherController = _root.Get<IPushController>();
            //    pusherController.SyncStop();
            //}
            //catch (Exception excp)
            //{
            //    logger.LogException(excp);
            //}

            //все выключаем
            _root.Dispose();
        }


        private static void CurrentDomain_UnhandledException(
            object sender,
            UnhandledExceptionEventArgs e
            )
        {
            var sb = new StringBuilder();
            sb.AppendLine(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss.fff"));

            var excp = e.ExceptionObject as Exception;
            if (excp != null)
            {
                LogException(1, sb, excp);
            }

            sb.AppendLine(new string('-', 80));

            var r = sb.ToString();

            File.AppendAllText("_CurrentDomain_UnhandledException.txt", r);
        }


        private static void LogException(
            int left,
            StringBuilder sb,
            Exception excp
            )
        {
            sb.AppendLine(new string(' ', left) + excp.Message);
            sb.AppendLine(new string(' ', left) + excp.StackTrace);
            sb.AppendLine();
            sb.AppendLine();

            if (excp.InnerException != null)
            {
                LogException(left + 8, sb, excp.InnerException);
            }
        }

    }
}
