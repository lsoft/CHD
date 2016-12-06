using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CHD.Installer.CompositionRoot;
using CHD.Installer.ViewModel;

namespace CHD.Installer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Root _root;

        public App(
            )
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            _root = new Root();
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            _root.Init();

            //готовим основное окно
            var mainWindow = _root.Get<View.MainWindow>();
            App.Current.MainWindow = mainWindow;

            var mvm = _root.Get<MainViewModel>();
            mainWindow.DataContext = mvm;

            mainWindow.Show();
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
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
