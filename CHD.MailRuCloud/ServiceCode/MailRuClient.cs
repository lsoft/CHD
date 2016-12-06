using System;
using System.Collections.Generic;
using System.Threading;
using CHD.Common.Logger;
using CHD.MailRuCloud.Operation;
using MailRu.Cloud.WebApi;
using MailRu.Cloud.WebApi.Connection;
using MailRu.Cloud.WebApi.Connection.Factory;

namespace CHD.MailRuCloud.ServiceCode
{
    public class MailRuClient : IDisposable
    {
        private readonly MailRuSettings _settings;
        private readonly IDisorderLogger _logger;

        private readonly ConnectionFactory _connectionFactory;

        public IConnection Connection
        {
            get;
            private set;
        }

        private int _disposed = 0;

        public MailRuClient(
            MailRuSettings settings,
            IDisorderLogger logger
            )
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _settings = settings;
            _logger = logger;

            _connectionFactory = new ConnectionFactory();
        }

        public void Connect()
        {
            Connection = _connectionFactory.OpenConnection(
                _settings.Login,
                _settings.Password
                );
        }

        public void Dispose()
        {
            var disposed = Interlocked.Exchange(ref _disposed, 1);
            if (disposed == 0)
            {
                //nothing to do
            }
        }


        public Dictionary<Guid, MailRuOperationFileNameContainer> ScanFiles(
            )
        {
            var result = new Dictionary<Guid, MailRuOperationFileNameContainer>();

            var items = Connection.GetItems(
                ServerPath.Root
                );

            foreach (var e in items.Files)
            {
                MailRuOperationFileNameContainer s;
                if (MailRuOperationFileNameContainer.TryParse(e.Name, out s))
                {
                    result.Add(s.MailGuid, s);
                }
            }

            return
                result;
        }

    }
}