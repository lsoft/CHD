using System;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
using CHD.Service.ArgProcessor;
using CHD.Service.WindowsService;

namespace CHD.Service
{
    internal sealed class Program
    {
        /// <summary>
        /// Инсталляция службы
        /// </summary>
        internal static Arg InstallArg = new Arg("-install", 0, 1);

        /// <summary>
        /// Деинсталляция службы
        /// </summary>
        internal static Arg UninstallArg = new Arg("-uninstall");

        /// <summary>
        /// Очистка локальной ФС, используется для отладки
        /// </summary>
        internal static Arg LocalClearArg = new Arg("-localclear");

        /// <summary>
        /// Очистка удаленной ФС, используется для отладки
        /// </summary>
        internal static Arg RemoteClearArg = new Arg("-remoteclear");

        /// <summary>
        /// Открыть консоль
        /// </summary>
        internal static Arg ConsoleArg = new Arg("-console", 0, 1);

        /// <summary>
        /// принудительно запускать несколько экземпляров
        /// </summary>
        internal static Arg ForceDuplicateArg = new Arg("-forceduplicate");

        /// <summary>
        /// Файл настроек
        /// </summary>
        internal static Arg SettingsArg = new Arg("-settings:", 0, 1);

        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var extractor = new ArgExtractor(args);

            extractor.Process(ref InstallArg);
            extractor.Process(ref UninstallArg);
            extractor.Process(ref ConsoleArg);
            extractor.Process(ref SettingsArg);
            extractor.Process(ref LocalClearArg);
            extractor.Process(ref RemoteClearArg);
            extractor.Process(ref ForceDuplicateArg);

            #region проверка, чтобы нельзя было бы запустить два экземпляра

            if (!ForceDuplicateArg.Exists)
            {
                var exefilename = typeof (Program).Module.Name.ToLower();
                var fi = new FileInfo(exefilename);
                var processname = exefilename.Substring(0, exefilename.Length - fi.Extension.Length);
                var processes = Process.GetProcesses().Where(j => j.ProcessName.ToLower() == processname);

                if (processes.Count() > 1)
                {
                    return;
                }
            }


            #endregion

            if (InstallArg.Exists && UninstallArg.Exists)
            {
                MessageBox.Show(
                    "Не допускается одновременно указывать ключ инсталляции и деинсталляции",
                    "Ошибка запуска",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }
            if (InstallArg.Exists && extractor.GetArgumentCount() != 1)
            {
                MessageBox.Show(
                    "Ключ инсталляции должен быть один",
                    "Ошибка запуска",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }
            if (UninstallArg.Exists && extractor.GetArgumentCount() != 1)
            {
                MessageBox.Show(
                    "Ключ деинсталляции должен быть один",
                    "Ошибка запуска",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }


            if (InstallArg.Exists || UninstallArg.Exists)
            {
                RegUnreg();

                return;
            }

            if (SettingsArg.Exists)
            {
                SettingsArg.AddTail("_servicesettings.xml");
            }

            var arguments = new Arguments(
                !ConsoleArg.Exists,
                SettingsArg.FirstTail,
                LocalClearArg.Exists,
                RemoteClearArg.Exists
                );

            if (ConsoleArg.Exists)
            {
                var ci = new ConsoleMode(
                    arguments
                    );
                ci.Execute();
            }
            else
            {
                ClientServiceIsolator si = null;

                if (Environment.OSVersion.Version.Major < 6)
                {
                    //что-то более раннее чем виста
                    //то есть это икспи, старее не может быть

                    si = new ServiceModeXP(
                        arguments
                        );
                }
                else
                {
                    si = new ServiceMode(
                        arguments
                        );
                }

                ServiceBase.Run(si);
            }
        }

        /// <summary>
        ///     Регистрация или отмена регистрации службы
        /// </summary>
        private static void RegUnreg()
        {
            // инициализируем консоль
            ConsoleTools.InitConsole(null);

            ConsoleTools.SetCloseButtonStatus(false);

            var textOut = "  ==[ Консольный режим, выполнение инсталлятора ]==".PadRight(Console.BufferWidth - 2, '=') + "  ";
            ConsoleTools.WriteToConsole(ConsoleColor.White, ConsoleColor.DarkYellow, textOut);

            try
            {
                var args = Environment.GetCommandLineArgs();

                ManagedInstallerClass.InstallHelper(args.Skip(1).Concat(new[] { Assembly.GetCallingAssembly().Location }).ToArray());
            }
            catch (Exception excp)
            {
                Console.WriteLine(excp.Message);
                Console.WriteLine("".PadLeft(10, '-'));
                Console.WriteLine(excp.StackTrace);
            }

            Console.Write("Нажмите любую клавишу для выхода....");
            Console.ReadKey();

        }


        #region обработка критических ошибок

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

        #endregion
    }
}





