using System;
using CHD.Common.Sync;
using CHD.Common.Sync.Report;
using CHD.Tests.Other;

namespace CHD.Tests.Sync
{
    public sealed class DebugLoggerSynchronizer : ISynchronizer
    {
        private readonly ISynchronizer _synchronizer;
        private readonly DebugLogger _logger;

        public DebugLoggerSynchronizer(
            ISynchronizer synchronizer,
            DebugLogger logger
            )
        {
            if (synchronizer == null)
            {
                throw new ArgumentNullException("synchronizer");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _synchronizer = synchronizer;
            _logger = logger;
        }

        public SyncReport Sync()
        {
            SyncReport result;

            _logger.SetStandardPrefix();
            try
            {
                result = _synchronizer.Sync();
            }
            finally
            {
                _logger.ResetPrefix();
            }

            return result;
        }
    }
}