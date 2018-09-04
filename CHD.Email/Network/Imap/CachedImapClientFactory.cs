using System;
using System.Threading;
using CHD.Common;
using MailKit;

namespace CHD.Email.Network.Imap
{
    public sealed class CachedImapClientFactory : IImapClientFactory, IDisposable
    {
        private const long ActiveStatus = 1L;
        private const long DisposeStatus = 2L;

        private readonly IDisorderLogger _logger;

        private readonly TimeCache<IImapConnectedClient> _cache;

        private long _status;

        public CachedImapClientFactory(
            IImapClientFactory clientFactory,
            int aliveTimeoutInSeconds,
            IDisorderLogger logger
            )
        {
            if (clientFactory == null)
            {
                throw new ArgumentNullException("clientFactory");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _logger = logger;

            _cache = new TimeCache<IImapConnectedClient>(
                () => clientFactory.CreateConnectedClient(),
                aliveTimeoutInSeconds
                );

            _status = ActiveStatus;
        }

        public IImapConnectedClient CreateConnectedClient()
        {
            if (Interlocked.Read(ref _status) != ActiveStatus)
            {
                return
                    null;
            }

            var client = _cache.GetOrCreate();
            if (client == null)
            {
                return
                    null;
            }

            var wrapper = new ImapConnectedClientWrapper(
                client,
                AddToCache
                );

            return
                wrapper;
        }

        public void Dispose()
        {
            var status = Interlocked.CompareExchange(ref _status, DisposeStatus, ActiveStatus);
            if (status == ActiveStatus)
            {
                DoDispose();
            }
        }

        private void DoDispose()
        {
            _cache.Dispose();
        }

        private void AddToCache(
            ImapConnectedClientWrapper wrapper
            )
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }

            _cache.AddToCache(
                wrapper.InnerClient
                );
        }


        internal sealed class ImapConnectedClientWrapper : IImapConnectedClient
        {
            private readonly IImapConnectedClient _client;
            private readonly Action<ImapConnectedClientWrapper> _sleepAction;

            private const long WActiveStatus = 1L;
            private const long WSleepStatus = 2L;

            private long _status;

            public IMailFolder Sent
            {
                get
                {
                    return
                        _client.Sent;
                }
            }

            public IImapConnectedClient InnerClient
            {
                get
                {
                    return
                        _client;
                }
            }

            public ImapConnectedClientWrapper(
                IImapConnectedClient client,
                Action<ImapConnectedClientWrapper> sleepAction
                )
            {
                if (client == null)
                {
                    throw new ArgumentNullException("client");
                }
                if (sleepAction == null)
                {
                    throw new ArgumentNullException("sleepAction");
                }
                _client = client;
                _sleepAction = sleepAction;

                _status = WActiveStatus;
            }

            public IMailFolder SafelyGetChildfolder(IMailFolder parent, string folderName)
            {
                var result = _client.SafelyGetChildfolder(parent, folderName);
                return result;
            }

            public IMailFolder CreateFolder(IMailFolder parent, string folderName)
            {
                var result = _client.CreateFolder(parent, folderName);
                return result;
            }

            public void Dispose()
            {
                var previousStatus = Interlocked.CompareExchange(ref _status, WSleepStatus, WActiveStatus);
                if (previousStatus == WActiveStatus)
                {
                    _sleepAction(this);
                }
            }

        }
    }
}