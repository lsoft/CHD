using System;
using System.IO;
using CHD.Common.Native;
using CHD.Common.Others;
using CHD.Common.PathComparer;
using CHD.Common.Saver;
using CHD.Common.ServiceCode;
using CHD.Common.ServiceCode.Executor;

namespace CHD.Common.Letter
{
    public sealed class Letter<TNativeMessage, TSendableMessage> : ILetter<TNativeMessage>
        where TNativeMessage : NativeMessage
        where TSendableMessage : SendableMessage
    {
        private readonly INativeClientExecutor<TNativeMessage, TSendableMessage> _executor;

        public TNativeMessage NativeMessage
        {
            get;
            private set;
        }

        public int StructureVersion
        {
            get;
            private set;
        }

        public long Order
        {
            get;
            private set;
        }

        public Guid TransactionGuid
        {
            get;
            private set;
        }

        public MessageTypeEnum MessageType
        {
            get;
            private set;
        }

        public PathSequence FullPathSequence
        {
            get;
            private set;
        }

        public Guid LetterGuid
        {
            get;
            private set;
        }

        public long Size
        {
            get;
            private set;
        }

        public Letter(
            IPathComparerProvider pathComparerProvider,
            INativeClientExecutor<TNativeMessage, TSendableMessage> executor,
            TNativeMessage nativeMessage
            )
        {
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }
            if (executor == null)
            {
                throw new ArgumentNullException("executor");
            }
            if (nativeMessage == null)
            {
                throw new ArgumentNullException("nativeMessage");
            }

            _executor = executor;
            NativeMessage = nativeMessage;

            int structureCurrentVersion;
            long order;
            Guid transactionGuid;
            MessageTypeEnum messageType;
            string fullPath;
            Guid letterGuid;
            long size;
            SubjectComposer.ParseSubject(
                nativeMessage.Subject,
                out structureCurrentVersion,
                out order,
                out transactionGuid,
                out messageType,
                out fullPath,
                out letterGuid,
                out size
                );

            if (messageType == MessageTypeEnum.BlockData && size == 0)
            {
                throw new CHDException(
                    string.Format("File have zero bytes body: {0}", fullPath),
                    CHDExceptionTypeEnum.EmptyFileBody
                    );
            }

            StructureVersion = structureCurrentVersion;
            Order = order;
            TransactionGuid = transactionGuid;
            MessageType = messageType;
            FullPathSequence = new PathSequence(pathComparerProvider, fullPath);
            LetterGuid = letterGuid;
            Size = size;
        }

        public long WriteAttachmentTo(
            Stream destination,
            long position,
            long size
            )
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            if (MessageType != MessageTypeEnum.BlockData)
            {
                throw new InvalidOperationException("MessageType != EmailMessageTypeEnum.BlockData");
            }

            long copyiedSize = 0L;
            _executor.Execute(
                client =>
                {
                    copyiedSize = client.DecodeAttachmentTo(
                        NativeMessage,
                        destination,
                        position,
                        size
                        );
                }
                );

            return
                copyiedSize;
        }

        //public void Delete(
        //    )
        //{
        //    _executor.Execute(
        //        client =>
        //        {
        //            client.DeleteMessage(Uid);
        //        });
        //}

    }
}
