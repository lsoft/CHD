using System;
using CHD.Common.FileSystem;
using CHD.Common.Sync;
using CHD.Common.Sync.Factory;
using CHD.Tests.Other;

namespace CHD.Tests.Sync
{
    public sealed class DebugLoggerSynchronizerFactory : ISynchronizerFactory
    {
        private readonly ISynchronizerFactory _factory;
        private readonly DebugLogger _logger;

        public DebugLoggerSynchronizerFactory(
            ISynchronizerFactory factory,
            DebugLogger logger
            )
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _factory = factory;
            _logger = logger;
        }

        public ISynchronizer CreateSynchronizer(
            IFileSystemConnector localConnector,
            IFileSystemConnector remoteConnector
            )
        {
            var isync = _factory.CreateSynchronizer(
                localConnector,
                remoteConnector
                );

            var dsync = new DebugLoggerSynchronizer(
                isync,
                _logger
                );

            return
                dsync;
        }
    }
}