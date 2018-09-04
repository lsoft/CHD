using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace CHD.Service
{
    public static class ConsoleTools
    {
        public delegate bool ConsoleCtrlHandlerRoutine(int controlType);


        private static ConsoleCtrlHandlerRoutine _consoleCtrlHandler;


        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();


        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();


        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleCtrlHandlerRoutine handler, bool addOrRemove);


        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);


        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        internal const UInt32 SC_CLOSE = 0xF060;
        internal const UInt32 MF_ENABLED = 0x00000000;
        internal const UInt32 MF_GRAYED = 0x00000001;
        internal const UInt32 MF_DISABLED = 0x00000002;
        internal const uint MF_BYCOMMAND = 0x00000000;

        [DllImport("kernel32.dll",
                   EntryPoint = "GetStdHandle",
                   SetLastError = true,
                   CharSet = CharSet.Auto,
                   CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        private const int STD_OUTPUT_HANDLE = -11;

        
        /// <summary>
        ///     Инициализация консоли
        /// </summary>
        public static void InitConsole(ConsoleCtrlHandlerRoutine consoleCtrlHandler)
        {
            if (GetConsoleWindow() == IntPtr.Zero)
            {
                AllocConsole();
                InvalidateOutAndError();
            }

            DisableCloseAndHook(consoleCtrlHandler);
        }

        /// <summary>
        /// Отключает или включает кнопку закрытия консоли
        /// </summary>
        /// <param name="bEnabled"></param>
        public static void SetCloseButtonStatus(
            bool bEnabled
            )
        {
            var cw = GetConsoleWindow();

            var hSystemMenu = GetSystemMenu(cw, false);

            EnableMenuItem(
                hSystemMenu,
                SC_CLOSE,
                (uint)(MF_ENABLED | (bEnabled ? MF_ENABLED : MF_GRAYED)));
        }

        /// <summary>
        ///     Запись в консоль текста с выбранными цветами фона и шрифта
        /// </summary>
        public static void WriteToConsole(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string text)
        {
            ConsoleColor originalFgColor = Console.ForegroundColor;
            ConsoleColor originalBgColor = Console.BackgroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.WriteLine(text);
            Console.Out.Flush();
            Console.ForegroundColor = originalFgColor;
            Console.BackgroundColor = originalBgColor;
        }


        /// <summary>
        ///     Инициализация переменных выходных потоков Out/Err в среде выполнения
        /// </summary>
        private static void InvalidateOutAndError()
        {
            IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
            FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            Encoding outputEncoding = Encoding.GetEncoding(866);
            StreamWriter standardOutput = new StreamWriter(fileStream, outputEncoding);
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);

            //var consoleType = typeof(Console);

            //var outFld = consoleType.GetField("_out", BindingFlags.Static | BindingFlags.NonPublic);
            //var errorFld = consoleType.GetField("_error", BindingFlags.Static | BindingFlags.NonPublic);

            //var initializeStdOutErrorMthd = consoleType.GetMethod(
            //    "InitializeStdOutError", BindingFlags.Static | BindingFlags.NonPublic);

            //Debug.Assert(outFld != null);
            //Debug.Assert(errorFld != null);

            //outFld.SetValue(null, null);
            //errorFld.SetValue(null, null);

            //Debug.Assert(initializeStdOutErrorMthd != null);

            //initializeStdOutErrorMthd.Invoke(null, new object[] { true });

            //var enc = Console.OutputEncoding;
        }


        /// <summary>
        ///     Выгрузка обработчика Ctrl+C
        /// </summary>
        private static void Unhook()
        {
            if (_consoleCtrlHandler != null)
            {
                SetConsoleCtrlHandler(_consoleCtrlHandler, false);
                _consoleCtrlHandler = null;
            }
        }


        /// <summary>
        ///     Загрузка обработчика Ctrl+C
        /// </summary>
        private static void DisableCloseAndHook(ConsoleCtrlHandlerRoutine consoleCtrlHandler)
        {
            if (_consoleCtrlHandler != null)
            {
                Unhook();
            }

            _consoleCtrlHandler = consoleCtrlHandler;

            /*
                    IntPtr hWnd = GetConsoleWindow();
                    GetSystemMenu(hWnd, true);
                    EnableMenuItem(GetSystemMenu(hWnd, false), 0xF060, 0x00000002 | 0x00000001);
            */

            if (consoleCtrlHandler != null)
            {
                SetConsoleCtrlHandler(_consoleCtrlHandler, true);
            }
        }
    }
}