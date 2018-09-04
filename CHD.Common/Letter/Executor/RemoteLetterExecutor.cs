using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CHD.Common.FileSystem;
using CHD.Common.FileSystem.FFile;
using CHD.Common.Letter.Container;
using CHD.Common.Letter.Container.Factory;
using CHD.Common.Letter.Factory;
using CHD.Common.Native;
using CHD.Common.Others;
using CHD.Common.Saver;
using CHD.Common.ServiceCode;
using CHD.Common.ServiceCode.Executor;

namespace CHD.Common.Letter.Executor
{
    public sealed class RemoteLetterExecutor<TNativeMessage, TSendableMessage> : ILetterExecutor<TNativeMessage>
        where TNativeMessage : NativeMessage
        where TSendableMessage : SendableMessage
    {
        private readonly IRemoteSettings _remoteSettings;
        private readonly ISendableMessageFactory<TSendableMessage> _sendableMessageFactory;
        private readonly INativeClientExecutor<TNativeMessage, TSendableMessage> _executor;
        private readonly ILetterFactory<TNativeMessage> _letterFactory;
        private readonly ILettersContainerFactory<TNativeMessage> _lettersContainerFactory;
        private readonly IDisorderLogger _logger;

        public RemoteLetterExecutor(
            IRemoteSettings remoteSettings,
            ISendableMessageFactory<TSendableMessage> sendableMessageFactory,
            INativeClientExecutor<TNativeMessage, TSendableMessage> executor,
            ILetterFactory<TNativeMessage> letterFactory,
            ILettersContainerFactory<TNativeMessage> lettersContainerFactory,
            IDisorderLogger logger
            )
        {
            if (remoteSettings == null)
            {
                throw new ArgumentNullException("remoteSettings");
            }
            if (sendableMessageFactory == null)
            {
                throw new ArgumentNullException("sendableMessageFactory");
            }
            if (executor == null)
            {
                throw new ArgumentNullException("executor");
            }
            if (letterFactory == null)
            {
                throw new ArgumentNullException("letterFactory");
            }
            if (lettersContainerFactory == null)
            {
                throw new ArgumentNullException("lettersContainerFactory");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _remoteSettings = remoteSettings;
            _sendableMessageFactory = sendableMessageFactory;
            _executor = executor;
            _letterFactory = letterFactory;
            _lettersContainerFactory = lettersContainerFactory;
            _logger = logger;
        }

        public ILettersContainer<TNativeMessage> ReadAllLetters(
            )
        {
            var letters = new List<ILetter<TNativeMessage>>();

            _executor.Execute(
                client =>
                {
                    foreach (var native in client.ReadAndFilterMessages(SubjectComposer.IsSubjectCanBeParsed))
                    {
                        var operation = _letterFactory.Create(
                            native
                            );

                        letters.Add(operation);
                    }
                });

            var result = _lettersContainerFactory.Create(
                letters
                );

            return result;
        }

        public void SaveFile(
            int structureCurrentVersion,
            long maxOrder,
            IFileSystemCopier copier,
            ICopyableFile sourceFile,
            IFile targetFile
            )
        {
            _executor.Execute(
                client =>
                {
                    SendOpenFileMessage(
                        structureCurrentVersion,
                        ref maxOrder,
                        client,
                        targetFile
                        );

                    SendFileBodyMessages(
                        structureCurrentVersion,
                        ref maxOrder,
                        client,
                        copier,
                        sourceFile,
                        targetFile
                        );

                    SendCloseFileMessage(
                        structureCurrentVersion,
                        ref maxOrder,
                        client,
                        targetFile
                        );
                });
        }

        public void DeleteSnapshots(
            IEnumerable<FileSnapshot<TNativeMessage>> snapshots
            )
        {
            if (snapshots == null)
            {
                throw new ArgumentNullException("snapshots");
            }

            var sb = new StringBuilder();

            var acc = new Dictionary<Guid, TNativeMessage>();

            foreach (var snapshot in snapshots.OrderBy(j => j.StructureVersion))
            {
                sb.AppendFormat(
                    "From snapshot with parameters:{0}    Path: {1}{0}    Version: {2}{0}    Min Order: {3}{0}    Max Order: {4}{0}    Size: {5}{0}    Transaction Guid: {6}{0}",
                    Environment.NewLine,
                    snapshot.FullPathSequence.Path,
                    snapshot.StructureVersion,
                    snapshot.MinOrder,
                    snapshot.MaxOrder,
                    snapshot.Size,
                    snapshot.TransactionGuid
                    );

                foreach (var letter in snapshot.Letters)
                {
                    if (!acc.ContainsKey(letter.LetterGuid))
                    {
                        acc.Add(
                            letter.LetterGuid,
                            letter.NativeMessage
                            );

                        sb.AppendLine(
                            string.Format(
                                "        Message type: {1}{0}        Mail Guid: {2}{0}",
                                Environment.NewLine,
                                letter.MessageType,
                                letter.LetterGuid
                                )
                            );
                    }
                }
            }

            _executor.Execute(
                client =>
                {
                    client.DeleteMessages(acc.Values);
                });

            _logger.LogFormattedMessage(
                "Letters deleted:{0}{1}",
                Environment.NewLine,
                sb
                );
        }

        public void DeleteSnapshot(
            FileSnapshot<TNativeMessage> snapshot
            )
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException("snapshot");
            }

            _executor.Execute(
                client =>
                {
                    client.DeleteMessages(snapshot.Messages);
                });
        }






        private void SendFileBodyMessages(
            int structureCurrentVersion,
            ref long maxOrder,
            INativeClientEx<TNativeMessage, TSendableMessage> client,
            IFileSystemCopier copier,
            ICopyableFile sourceFile,
            IFile targetFile
            )
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }
            if (copier == null)
            {
                throw new ArgumentNullException("copier");
            }
            if (sourceFile == null)
            {
                throw new ArgumentNullException("sourceFile");
            }
            if (targetFile == null)
            {
                throw new ArgumentNullException("targetFile");
            }


            var position = 0L;
            var size = sourceFile.Size;

            while (size > 0L)
            {
                long copiedSize;
                using (var ms = new MemoryStream())
                {
                    var sizeToCopy = Math.Min(size, _remoteSettings.MaxFileBlockSize);

                    copiedSize = copier.CopyFileTo(
                        sourceFile,
                        ms,
                        position,
                        sizeToCopy
                        );

                    ms.Position = 0;

                    maxOrder++;

                    var message = _sendableMessageFactory.CreateRegularMessage(
                        structureCurrentVersion,
                        maxOrder,
                        targetFile.ChangeIdentifier,
                        MessageTypeEnum.BlockData,
                        targetFile,
                        ms
                        );

                    client.StoreMessage(
                        message
                        );

                    position += copiedSize;
                    size -= copiedSize;
                }
            }
        }

        private void SendCloseFileMessage(
            int structureCurrentVersion,
            ref long maxOrder,
            INativeClientEx<TNativeMessage, TSendableMessage> client,
            IFile targetFile
            )
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }
            if (targetFile == null)
            {
                throw new ArgumentNullException("targetFile");
            }

            maxOrder++;

            var message = _sendableMessageFactory.CreateRegularMessage(
                structureCurrentVersion,
                maxOrder,
                targetFile.ChangeIdentifier,
                MessageTypeEnum.CloseFile,
                targetFile,
                null
                );

            client.StoreMessage(
                message
                );
        }

        private void SendOpenFileMessage(
            int structureCurrentVersion,
            ref long maxOrder,
            INativeClientEx<TNativeMessage, TSendableMessage> client,
            IFile targetFile
            )
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }
            if (targetFile == null)
            {
                throw new ArgumentNullException("targetFile");
            }

            maxOrder++;

            var message = _sendableMessageFactory.CreateRegularMessage(
                structureCurrentVersion,
                maxOrder,
                targetFile.ChangeIdentifier,
                MessageTypeEnum.OpenFile,
                targetFile,
                null
                );

            client.StoreMessage(
                message
                );
        }
    }
}