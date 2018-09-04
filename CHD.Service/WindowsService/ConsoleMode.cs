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
            
            // �������������� �������
            ConsoleTools.InitConsole(this.ConsoleCtrlHandler);

            ConsoleTools.SetCloseButtonStatus(false);
            
            var textOut =
                "  ==[ ���������� �����, ������� Q/Ctrl+C ��� ������ ]==".PadRight(Console.BufferWidth - 2, '=')
                + "  ";

            ConsoleTools.WriteToConsole(ConsoleColor.White, ConsoleColor.DarkRed, textOut);
        }

        /// <summary>
        /// ���������� ����������
        /// </summary>
        public void Execute()
        {
            var cs = new ClientStarter(
                _arguments
                );

            try
            {
                cs.Start();

                Console.WriteLine("������ ���������...");
                Console.WriteLine("������� �� Ctrl+C �������� ���������...");
                
                _mre.WaitOne();
            }
            finally
            {
                cs.SyncStop();

                _mre.Dispose();
            }
        }

        /// <summary>
        ///     ���������� ������� Ctrl+C
        /// </summary>
        private bool ConsoleCtrlHandler(int controlType)
        {
            _mre.Set();

            Thread.Sleep(250);

            return true;
        }

    }
}