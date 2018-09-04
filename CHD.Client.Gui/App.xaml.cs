using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CHD.Client.Gui.CompositionRoot;
using CHD.Client.Gui.CompositionRoot.Helper;
using CHD.Client.Gui.ViewModel;
using CHD.Client.Gui.ViewModel.Main;
using CHD.Common;

namespace CHD.Client.Gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Root2 _root;

        public App(
            )
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var arguments = new Arguments(
                "_appsettings.xml"
                );

            _root = new Root2(
                arguments
                );
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            _root.BindAll();

            var windowFactory = _root.Get<IMainWindowFactory>();
            var mainWindow = windowFactory.Create();
            App.Current.MainWindow = mainWindow;
            App.Current.MainWindow.Show();

            _root.Start();
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            //все выключаем

            _root.Stop();
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
