using System;
using System.Threading;

namespace CHD.Service.WindowsService
{
    internal sealed class ConsoleMode
    {
        private readonly Arguments _arguments;
        private readonly ManualResetEvent _mre = new ManualResetEvent(false);

        public ConsoleMode(
            Arguments arguments
            )
        {
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }

            _arguments = arguments;
            
            // инициализируем консоль
            ConsoleTools.InitConsole(this.ConsoleCtrlHandler);

            ConsoleTools.SetCloseButtonStatus(false);
            
            var textOut =
                "  ==[ Консольный режим, нажмите Q/Ctrl+C для выхода ]==".PadRight(Console.BufferWidth - 2, '=')
                + "  ";

            ConsoleTools.WriteToConsole(ConsoleColor.White, ConsoleColor.DarkRed, textOut);
        }

        /// <summary>
        /// Синхронное выполнение
        /// </summary>
        public void Execute()
        {
            var cs = new ClientStarter(
                _arguments
                );

            try
            {
                cs.Start();

                Console.WriteLine("Клиент стартовал...");
                Console.WriteLine("Нажатие на Ctrl+C завершит программу...");
                
                _mre.WaitOne();
            }
            finally
            {
                cs.SyncStop();

                _mre.Dispose();
            }
        }

        /// <summary>
        ///     Обработчик нажатия Ctrl+C
        /// </summary>
        private bool ConsoleCtrlHandler(int controlType)
        {
            _mre.Set();

            Thread.Sleep(250);

            return true;
        }

    }
}