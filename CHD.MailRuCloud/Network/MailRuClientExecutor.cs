using System;
using CHD.Common;
using CHD.Common.PathComparer;
using CHD.Common.ServiceCode.Executor;
using CHD.MailRuCloud.Native;
using CHD.MailRuCloud.Settings;
using MailRu.Cloud.WebApi.Connection.Factory;

namespace CHD.MailRuCloud.Network
{
    public sealed class MailRuClientExecutor : INativeClientExecutor<MailRuNativeMessage, MailRuSendableMessage>
    {
        private readonly MailRuSettings _settings;
        private readonly IPathComparerProvider _pathComparerProvider;
        private readonly IMailRuConnectionFactory _connectionFactory;
        private readonly IDisorderLogger _logger;

        public MailRuClientExecutor(
            MailRuSettings settings,
            IPathComparerProvider pathComparerProvider,
            IMailRuConnectionFactory connectionFactory,
            IDisorderLogger logger
            )
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }
            if (connectionFactory == null)
            {
                throw new ArgumentNullException("connectionFactory");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _settings = settings;
            _pathComparerProvider = pathComparerProvider;
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public void Execute(
            Action<INativeClientEx<MailRuNativeMessage, MailRuSendableMessage>> executeAction
            )
        {
            if (executeAction == null)
            {
                throw new ArgumentNullException("executeAction");
            }

            using (var connection = _connectionFactory.OpenConnection(_settings.Login, _settings.Password))
            {
                var clientex = new MailRuNativeClientEx(_pathComparerProvider, connection, _logger);
                
                executeAction(clientex);
            }
        }

        public T Execute<T>(
            Func<INativeClientEx<MailRuNativeMessage, MailRuSendableMessage>, T> executeAction
            )
        {
            if (executeAction == null)
            {
                throw new ArgumentNullException("executeAction");
            }

            using (var connection = _connectionFactory.OpenConnection(_settings.Login, _settings.Password))
            {
                var clientex = new MailRuNativeClientEx(_pathComparerProvider, connection, _logger);

                var result = executeAction(clientex);

                return
                    result;
            }
        }
    }
}