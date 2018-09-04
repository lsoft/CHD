using System;

namespace CHD.Common.Operation
{
    public sealed class LoggerOperationDumper : IOperationDumper
    {
        private readonly IDisorderLogger _logger;

        public LoggerOperationDumper(
            IDisorderLogger logger
            )
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _logger = logger;
        }

        public void LogOperation(OperationTypeEnum type, string fullPath)
        {
            if (fullPath == null)
            {
                throw new ArgumentNullException("fullPath");
            }

            _logger.LogFormattedMessage(
                "{0}: {1}",
                type,
                fullPath
                );
        }
    }
}