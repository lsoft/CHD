using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CHD.Common.Logger;
using CHD.MailRuCloud.Operation;
using MailRu.Cloud.WebApi;

namespace CHD.MailRuCloud.ServiceCode
{
    public class MailRuClientEx : IDisposable
    {
        private readonly MailRuSettings _settings;
        private readonly IDisorderLogger _logger;

        private int _disposed = 0;
        
        private readonly MailRuClient _client;

        public MailRuClientEx(
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

            _client = new MailRuClient(
                settings,
                logger
                );

            try
            {
                _client.Connect();
            }
            catch
            {
                _client.Dispose();
                throw;
            }
        }

        public bool IsSubfolderExists(string tokenFolder)
        {
            var result = false;

            try
            {
                var entry = _client.Connection.GetItems(
                    ServerPath.Root
                    );

                result = entry.Folders.Any(j => j.Name == tokenFolder);
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);
            }

            return
                result;
        }

        public string CreateFolder(string tokenFolder)
        {
            var createdFolderPath = _client.Connection.CreateFolder(
                tokenFolder,
                ServerPath.Root
                );

            var result = createdFolderPath.GetPath().Substring(1);

            return
                result;
        }

        public string DeleteFolder(string tokenFolder)
        {
            var deletedFolderPath = _client.Connection.RemoveFolder(
                new ServerPath(tokenFolder)
                );

            var result = deletedFolderPath.GetPath().Substring(1);

            return
                result;
        }

        public List<MailRuOperationFileNameContainer> Scan()
        {
            var result = new List<MailRuOperationFileNameContainer>();

            try
            {
                var messages = _client.ScanFiles();

                result.AddRange(messages.Values);
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);
            }

            return
                result;
        }

        public void DecodeAttachmentTo(
            Guid mailGuid,
            Stream destination
            )
        {
            try
            {
                var messages = _client.ScanFiles();

                MailRuOperationFileNameContainer message;
                if (messages.TryGetValue(mailGuid, out message))
                {
                    _client.Connection.DownloadFile(
                        destination,
                        ServerPath.Root.Combine(message.FileName)
                        );
                }
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);
            }
        }

        public void DeleteMessage(
            Guid mailGuid
            )
        {
            try
            {
                var messages = _client.ScanFiles();

                MailRuOperationFileNameContainer message;
                if (messages.TryGetValue(mailGuid, out message))
                {
                    _client.Connection.RemoveFile(
                        ServerPath.Root.Combine(message.FileName)
                        );
                }
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);
            }
        }

        public void AppendMessage(
            MailRuOperationFileNameContainer fileNameContainer,
            byte[] data
            )
        {
            try
            {
                using (var ms = new MemoryStream(data ?? new byte[0]))
                {
                    _client.Connection.UploadFile(
                        ms,
                        fileNameContainer.FileName,
                        ServerPath.Root
                        );
                }
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);
            }
        }

        public void Dispose()
        {
            var disposed = Interlocked.Exchange(ref _disposed, 1);
            if (disposed == 0)
            {
                _client.Dispose();
            }
        }
    }
}
