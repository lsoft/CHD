using System;
using System.Threading;
using CHD.Common.Logger;
using CHD.FileSystem.FileWrapper;
using CHD.Graveyard.Operation;

namespace CHD.Graveyard.RemoteFile
{
    public class RemoteFile : IRemoteFile
    {
        private readonly IOperationContainer _operationContainer;
        private readonly IOperationFactory _operationFactory;
        private readonly Guid _transactionGuid;
        private readonly IFileWrapper _fileWrapper;
        private readonly Action _closeAction;
        private readonly IDisorderLogger _logger;

        private int _closed = 0;

        public RemoteFile(
            IOperationContainer operationContainer,
            IOperationFactory operationFactory,
            Guid transactionGuid,
            IFileWrapper fileWrapper,
            Action closeAction,
            IDisorderLogger logger
            )
        {
            if (operationContainer == null)
            {
                throw new ArgumentNullException("operationContainer");
            }
            if (operationFactory == null)
            {
                throw new ArgumentNullException("operationFactory");
            }
            if (fileWrapper == null)
            {
                throw new ArgumentNullException("fileWrapper");
            }
            if (closeAction == null)
            {
                throw new ArgumentNullException("closeAction");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _operationContainer = operationContainer;
            _operationFactory = operationFactory;
            _transactionGuid = transactionGuid;
            _fileWrapper = fileWrapper;
            _closeAction = closeAction;
            _logger = logger;

            _logger.LogFormattedMessage(
                "Start uploading file {0}",
                fileWrapper.FilePath
                );
        }

        public void StoreBlock(
            byte[] data
            )
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            PerformStoreBlockOperation(data);
        }

        public void Close()
        {
            var closed = Interlocked.Exchange(ref _closed, 1);

            if (closed == 0)
            {
                PerformCloseFileOperation();

                _closeAction();

                _logger.LogFormattedMessage(
                    "Completed uploading file {0}",
                    _fileWrapper.FilePath
                    );
            }
        }

        private void PerformStoreBlockOperation(byte[] data)
        {
            var nextOrder = _operationContainer.LastOrder + 1;

            var cbdo = _operationFactory.CreateBlockDataOperation(
                _transactionGuid,
                nextOrder,
                _fileWrapper,
                data
                );

            _operationContainer.Add(cbdo);

            _logger.LogFormattedMessage(
                "Block uploaded: {0}",
                _fileWrapper.FilePath
                );
        }

        private void PerformCloseFileOperation()
        {
            var nextOrder = _operationContainer.LastOrder + 1;

            var cfo = _operationFactory.CreateCloseFileOperation(
                _transactionGuid,
                nextOrder,
                _fileWrapper
                );

            _operationContainer.Add(cfo);
        }
    }
}