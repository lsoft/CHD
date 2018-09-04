using System;
using System.Diagnostics;
using System.Threading;
using CHD.Common;
using PerformanceTelemetry;

namespace CHD.Tests.Other
{
    public sealed class DebugLogger : IDisorderLogger, ITelemetryLogger
    {
        private bool _prefixed = false;

        private string _prefix;
        private string _nprefix;

        public DebugLogger()
        {
            _prefix = string.Empty;
            _nprefix = string.Empty;
            _prefixed = false;
        }

        #region IDisorderLogger

        public void LogDateDetailed(DateTime dt, string messageWithMask = "")
        {
            var mwm = !string.IsNullOrEmpty(messageWithMask) ? messageWithMask : "{0}";

            Debug.WriteLine(
                Prepare(mwm),
                new []
                {
                    dt.ToString("dd.MM.yyyy HH:mm:ss.fff")
                }
                );
        }

        public void LogMessage(string message)
        {
            Debug.WriteLine(Prepare(message));
        }

        public void LogFormattedMessage(string message, params object[] args)
        {
            ProcessArgs(args);

            Debug.WriteLine(Prepare(message), args);
        }

        public void LogException(Exception excp, string message = "")
        {
            Debug.WriteLine(Prepare(message));
            Debug.WriteLine(Prepare(excp.Message));
            Debug.WriteLine(Prepare(excp.StackTrace));
        }

        #endregion

        #region ITelemetryLogger

        public void LogMessage(Type sourceType, string message)
        {
            if (sourceType == null)
            {
                throw new ArgumentNullException("sourceType");
            }
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            Debug.WriteLine(
                "{0}:{1}{2}{2}{2}",
                sourceType.FullName,
                message,
                Environment.NewLine
                );
        }

        public void LogHandledException(Type sourceType, string message, Exception excp)
        {
            Console.WriteLine(
                "{0}:{1}{4}{2}{4}{3} {4}{4}{4}",
                sourceType.FullName,
                message,
                excp.Message,
                excp.StackTrace,
                Environment.NewLine
                );
        }

        #endregion

        #region other

        public void ResetPrefix(
            )
        {
            Volatile.Write(ref _prefixed, false);
        }

        public void SetStandardPrefix(
            )
        {
            SetPrefix(16, ' ');
        }

        public void SetPrefix(
            int prefixCount,
            char prefixChar
            )
        {
            _prefix = new string(prefixChar, prefixCount);
            _nprefix = "\n" + _prefix;
            Thread.MemoryBarrier();
            Volatile.Write(ref _prefixed, true);
        }

        private string Prepare(string p)
        {
            if (!Volatile.Read(ref _prefixed))
            {
                return p;
            }

            var result = _prefix + p.Replace("\n", _nprefix);

            return result;
        }

        private void ProcessArgs(object[] args)
        {
            if (!Volatile.Read(ref _prefixed))
            {
                return;
            }

            if (args != null)
            {
                for (var cc = 0; cc < args.Length; cc++)
                {
                    var argscc = args[cc] as string;
                    if (argscc != null)
                    {
                        args[cc] = argscc.Replace("\n", _nprefix);
                    }
                }
            }
        }

        #endregion
    }
}
