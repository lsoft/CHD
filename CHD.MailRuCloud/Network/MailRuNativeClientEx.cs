using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CHD.Common;
using CHD.Common.Others;
using CHD.Common.PathComparer;
using CHD.Common.ServiceCode.Executor;
using CHD.MailRuCloud.Native;
using MailRu.Cloud.WebApi;
using MailRu.Cloud.WebApi.Connection;

namespace CHD.MailRuCloud.Network
{

    public sealed class MailRuNativeClientEx : INativeClientEx<MailRuNativeMessage, MailRuSendableMessage>
    {
        private readonly IDisorderLogger _logger;

        private readonly IPathComparerProvider _pathComparerProvider;
        private readonly IMailRuConnection _client;
        private readonly MailRuCloudPath _folder;


        public MailRuNativeClientEx(
            IPathComparerProvider pathComparerProvider,
            IMailRuConnection client,
            IDisorderLogger logger
            )
        {
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _pathComparerProvider = pathComparerProvider;
            _client = client;
            _folder = MailRuCloudPath.Root;
            _logger = logger;
        }

        private MailRuNativeClientEx(
            IPathComparerProvider pathComparerProvider,
            IMailRuConnection client,
            MailRuCloudPath folder,
            IDisorderLogger logger
            )
        {
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }
            if (folder == null)
            {
                throw new ArgumentNullException("folder");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _pathComparerProvider = pathComparerProvider;
            _client = client;
            _folder = folder;
            _logger = logger;
        }

        public bool TryGetChild(
            string folderName,
            out INativeClientEx<MailRuNativeMessage, MailRuSendableMessage> client
            )
        {
            var result = false;
            client = null;

            var folder = SafelyGetChildfolder(
                folderName
                );

            if (folder != null)
            {
                result = true;

                client = new MailRuNativeClientEx(
                    _pathComparerProvider,
                    _client,
                    folder,
                    _logger
                    );
            }

            return
                result;
        }

        public INativeClientEx<MailRuNativeMessage, MailRuSendableMessage> CreateOrEnterChild(
            string folderName
            )
        {
            INativeClientEx<MailRuNativeMessage, MailRuSendableMessage> result = null;
            
            var folder = SafelyGetChildfolder(
                folderName
                );

            if (folder == null)
            {
                folder = DoCreateFolder(folderName);
            }

            result = new MailRuNativeClientEx(
                _pathComparerProvider,
                _client,
                folder,
                _logger
                );

            return
                result;
        }

        public void StoreMessage(
            MailRuSendableMessage message
            )
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            using (var ms = new MemoryStream(message.Data ?? new byte[0]))
            {
                _client.UploadFile(
                    ms,
                    message.Name,
                    _folder
                    );
            }

            _logger.LogFormattedMessage(
                "Message '{1}' sent to folder '{0}'",
                _folder.GetPath(),
                message.Name
                );
        }

        public void DeleteMessages(
            IEnumerable<MailRuNativeMessage> messages
            )
        {
            if (messages == null)
            {
                throw new ArgumentNullException("messages");
            }

            foreach (var fileName in messages.Select(m => m.File.Name).Distinct())
            {
                _client.RemoveFile(
                    _folder.Combine(fileName)
                    );
            }
        }

        public MailRuCloudPath SafelyGetChildfolder(
            string folderName
            )
        {
            MailRuCloudPath result = null;

            try
            {
                var entry = _client.GetItems(
                    _folder
                    );

                var childFolder = entry.Folders.FirstOrDefault(j => _pathComparerProvider.Comparer.Compare(j.Name, folderName) == 0);
                if (childFolder != null)
                {
                    result = childFolder.FullPath;
                }

                //var childFolder = entry.Folders.FirstOrDefault(j => _pathComparerProvider.Comparer.Compare(j.Name, folderName) == 0);
                //if (childFolder == null)
                //{
                //    result = DoCreateChildFolder(folderName);
                //}
                //else
                //{
                //    result = childFolder.FullPath;
                //}
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);
            }

            return
                result;
        }

        public bool IsChildFolderExists(string folderName)
        {
            var result = false;

            try
            {
                var entry = _client.GetItems(
                    _folder
                    );

                result = entry.Folders.Any(j => _pathComparerProvider.Comparer.Compare(j.Name, folderName) == 0);
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);
            }

            return
                result;
        }

        public void CreateChildFolder(
            string folderName,
            out string createdFolderName
            )
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName");
            }

            var createdFolder = DoCreateChildFolder(folderName);
            createdFolderName = createdFolder.GetLastElement();
        }

        private MailRuCloudPath DoCreateChildFolder(string folderName)
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName");
            }

            var folder = DoCreateFolder(folderName);

            if (folder == null)
            {
                throw new CHDException(
                    string.Format("Cannot create folder {0}", folderName),
                    CHDExceptionTypeEnum.CannotCreateFolder
                    );
            }

            return
                folder;
        }

        public bool DeleteChildFolder(
            string folderName,
            out string deletedFolderName
            )
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName");
            }

            var result = false;
            
            var resultFolder = _client.RemoveFolder(
                _folder.Combine(folderName)
                );

            result = true;
            deletedFolderName = resultFolder.GetLastElement();

            _logger.LogFormattedMessage(
                "Folder '{0}' deleted",
                folderName
                );

            return
                result;
        }

        public List<MailRuNativeMessage> ReadAndFilterMessages(
            Func<string, bool> filter
            )
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }

            var messages = new List<MailRuNativeMessage>();
            
            foreach (var file in _client.GetItems(_folder).Files)
            {
                var subject = file.Name;

                if (filter(subject))
                {
                    messages.Add(new MailRuNativeMessage(file));
                }
            }

            return messages;
        }

        public long DecodeAttachmentTo(
            MailRuNativeMessage message,
            Stream destination,
            long position = 0,
            long size = 0
            )
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }
            if (!destination.CanWrite)
            {
                throw new ArgumentException("!destination.CanWrite");
            }

            try
            {
                using (var tempbuffer = new MemoryStream())
                {
                    _client.DownloadFile(
                        tempbuffer,
                        _folder.Combine(message.File.Name)
                        );

                    if (size == 0)
                    {
                        size = tempbuffer.Position;
                    }

                    _logger.LogFormattedMessage(
                        "Decoded {0} bytes from message '{1}'",
                        tempbuffer.Position,
                        message.Subject
                        );

                    tempbuffer.Position = position;

                    var copiedSize = tempbuffer.CopyToConstraint(destination, size);

                    _logger.LogFormattedMessage(
                        "Copied {0} bytes from message '{1}'",
                        copiedSize,
                        message.Subject
                        );

                    return copiedSize;
                }
            }
            catch (Exception excp)
            {
                throw new CHDException(
                    string.Format("MailRu error with the file: {0}{1}", message.File.FullPath, Environment.NewLine),
                    excp,
                    CHDExceptionTypeEnum.FileDownloadError
                    );
            }
        }

        private MailRuCloudPath DoCreateFolder(string folderName)
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName");
            }

            var result = _client.CreateFolder(
                folderName,
                _folder
                );

            return
                result;
        }
    }
}