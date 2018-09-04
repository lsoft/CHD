using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHD.Common;
using XLogger.Logger;

namespace CHD.Logger
{
    public sealed class DisorderLogger : IDisorderLogger
    {
        private readonly IMessageLogger _logger;

        public DisorderLogger(
            IMessageLogger logger
            )
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _logger = logger;
        }

        public void LogFormattedMessage(string message, params object[] parameters)
        {
            if (parameters == null)
            {
                LogMessage(message);
            }

            _logger.LogMessage(
                string.Format(
                    message,
                    parameters
                )
            );
        }

        public void LogDateDetailed(DateTime dt, string messageWithMask = "")
        {
            _logger.LogMessage(
                string.Format(
                    !string.IsNullOrEmpty(messageWithMask) ? messageWithMask : "{0}",
                    dt.ToString("dd.MM.yyyy HH:mm:ss.fff")
                    )
                );
        }

        public void LogMessage(string message)
        {
            _logger.LogMessage(
                message
                );
        }

        public void LogException(Exception excp, string message = "")
        {
            _logger.LogException(
                excp,
                message
                );
        }
    }
}
