using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CHD.Common;
using CHD.MailRuCloud.Settings;
using MailRu.Cloud.WebApi;
using MailRu.Cloud.WebApi.Connection;
using MailRu.Cloud.WebApi.Connection.Factory;

namespace CHD.MailRuCloud.Network
{
    public sealed class CachedMailRuConnectionFactory : IMailRuConnectionFactory, IDisposable
    {
        private const long ActiveStatus = 1L;
        private const long DisposeStatus = 2L;

        private readonly IDisorderLogger _logger;

        private readonly TimeCache<IMailRuConnection> _cache;

        private long _status;

        public CachedMailRuConnectionFactory(
            MailRuSettings settings,
            IMailRuConnectionFactory connectionFactory,
            int aliveTimeoutInSeconds,
            IDisorderLogger logger
            )
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (connectionFactory == null)
            {
                throw new ArgumentNullException("connectionFactory");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }


            _logger = logger;

            _cache = new TimeCache<IMailRuConnection>(
                () => connectionFactory.OpenConnection(settings.Login, settings.Password),
                aliveTimeoutInSeconds
                );

            _status = ActiveStatus;
        }

        public IMailRuConnection OpenConnection(
            string login,
            string password
            )
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

            var wrapper = new MailRuConnectionWrapper(
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
            MailRuConnectionWrapper wrapper
            )
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }

            _cache.AddToCache(wrapper.InnerClient);
        }

        internal sealed class MailRuConnectionWrapper : IMailRuConnection
        {
            private readonly IMailRuConnection _client;
            private readonly Action<MailRuConnectionWrapper> _sleepAction;

            private const long WActiveStatus = 1L;
            private const long WSleepStatus = 2L;

            private long _status;

            public IMailRuConnection InnerClient
            {
                get
                {
                    return
                        _client;
                }
            }

            public MailRuConnectionWrapper(
                IMailRuConnection client,
                Action<MailRuConnectionWrapper> sleepAction
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

            /// <summary>
            ///     Create folder on the server.
            /// </summary>
            /// <param name="name">New path name.</param>
            /// <param name="createIn">Destination path.</param>
            public MailRuCloudPath CreateFolder(
                string name,
                MailRuCloudPath createIn
                )
            {
                return
                    _client.CreateFolder(
                        name,
                        createIn
                        );
            }

            /// <summary>
            ///     Remove folder.
            /// </summary>
            /// <param name="folderPath">Full folder name.</param>
            public MailRuCloudPath RemoveFolder(
                MailRuCloudPath folderPath
                )
            {
                return
                    _client.RemoveFolder(
                        folderPath
                        );
            }

            /// <summary>
            ///     Remove file.
            /// </summary>
            /// <param name="filePath">Full file name.</param>
            public void RemoveFile(
                MailRuCloudPath filePath
                )
            {
                _client.RemoveFolder(
                    filePath
                    );
            }

            /// <summary>
            ///     Upload file on the server asynchronously, if not use async await will be use synchronously operation.
            /// </summary>
            /// <param name="fileStream">file stream</param>
            /// <param name="fileName">File name.</param>
            /// <param name="destinationPath">Destination file path on the server.</param>
            public void UploadFile(
                Stream fileStream,
                string fileName,
                MailRuCloudPath destinationPath
                )
            {
                _client.UploadFile(
                    fileStream,
                    fileName,
                    destinationPath
                    );
            }

            /// <summary>
            ///     Download file 
            /// </summary>
            /// <param name="destination">channel to write file's data</param>
            /// <param name="filePath">Path to the file in the cloud</param>
            /// <returns>File as byte array.</returns>
            public void DownloadFile(
                Stream destination,
                MailRuCloudPath filePath
                )
            {
                _client.DownloadFile(
                    destination,
                    filePath
                    );
            }

            /// <summary>
            ///     Get list of files and folders from account.
            /// </summary>
            /// <param name="path">Path in the cloud to return the list of the items.</param>
            /// <returns>List of the items.</returns>
            public Entry GetItems(
                MailRuCloudPath path
                )
            {
                return
                    _client.GetItems(
                        path
                        );
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
