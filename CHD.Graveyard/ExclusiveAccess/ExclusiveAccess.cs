using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CHD.Common;
using CHD.Common.KeyValueContainer.Order;
using CHD.Common.Logger;
using CHD.FileSystem.FileWrapper;
using CHD.Graveyard.Operation;
using CHD.Graveyard.RemoteFile;
using CHD.Graveyard.RemoteFileState;

namespace CHD.Graveyard.ExclusiveAccess
{
    public class ExclusiveAccess : IExclusiveAccess
    {
        private readonly IOperationContainer _operationContainer;
        private readonly IOperationFactory _operationFactory;
        private readonly IOrderContainer _orderContainer;
        private readonly Action _closeAction;
        private readonly IDisorderLogger _logger;

        private int _closed = 0;

        private int _openFilesCount = 0;
        
        public ExclusiveAccess(
            IOperationContainerFactory operationContainerFactory,
            IOperationFactory operationFactory,
            IOrderContainer orderContainer,
            Action closeAction,
            IDisorderLogger logger
            )
        {
            if (operationContainerFactory == null)
            {
                throw new ArgumentNullException("operationContainerFactory");
            }
            if (operationFactory == null)
            {
                throw new ArgumentNullException("operationFactory");
            }
            if (orderContainer == null)
            {
                throw new ArgumentNullException("orderContainer");
            }
            if (closeAction == null)
            {
                throw new ArgumentNullException("closeAction");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _operationContainer = operationContainerFactory.OpenContainer();
            _operationFactory = operationFactory;
            _orderContainer = orderContainer;
            _closeAction = closeAction;
            _logger = logger;

            _operationContainer.Cleanup();
        }

        public bool ContainsTransaction(
            Guid transactionGuid
            )
        {
            return
                _operationContainer.ContainsTransaction(transactionGuid);
        }

        public IRemoteFile OpenRemoteFile(
            Guid transactionGuid,
            IFileWrapper fileWrapper,
            long size
            )
        {
            if (fileWrapper == null)
            {
                throw new ArgumentNullException("fileWrapper");
            }

            IRemoteFile result = null;

            if (!_operationContainer.ContainsTransaction(transactionGuid))
            {
                var nextOrder = _operationContainer.LastOrder + 1;

                var openTransaction = _operationFactory.CreateOpenFileOperation(
                    transactionGuid,
                    nextOrder,
                    fileWrapper
                    );

                result = new RemoteFile.RemoteFile(
                    _operationContainer,
                    _operationFactory,
                    transactionGuid,
                    fileWrapper,
                    () => Interlocked.Decrement(ref _openFilesCount),
                    _logger
                    );

                _operationContainer.Add(openTransaction);

                Interlocked.Increment(ref _openFilesCount);
            }

            return
                result;
        }

        public void DeleteRemoteFile(
            Guid transactionGuid,
            IFileWrapper fileWrapper
            )
        {
            if (fileWrapper == null)
            {
                throw new ArgumentNullException("fileWrapper");
            }

            if (!_operationContainer.ContainsTransaction(transactionGuid))
            {
                var nextOrder = _operationContainer.LastOrder + 1;

                var deleteOperation = _operationFactory.CreateDeleteFileOperation(
                    transactionGuid,
                    nextOrder,
                    fileWrapper
                    );

                _operationContainer.Add(deleteOperation);
            }
        }

        public List<IRemoteFileState> GetSnapshotSince(long lastOrder)
        {
            var result = new List<IRemoteFileState>();

            List<IOperation> operationsSince = _operationContainer.GetOperationsSince(lastOrder);

            var groups =
                from o in operationsSince 
                group o by o.FilePathSuffix.Key into go
                select
                    new
                    {
                        Operations = go.ToList()
                    }
                ;

            var ops = new[]
            {
                GraveyardOperationTypeEnum.OpenFile,
                GraveyardOperationTypeEnum.DeleteFile
            };

            foreach (var group in groups)
            {
                var lastOperationIndex = group.Operations.ConvertAll(j => j.OperationType.In(ops)).LastIndexOf(true);

                var rfs = new RemoteFileState.RemoteFileState(
                    group.Operations.Skip(lastOperationIndex).ToList(),
                    _logger
                    );

                result.Add(rfs);
            }

            return
                result;
        }

        public void Close()
        {
            var openFilesCount = _openFilesCount;

            if (openFilesCount > 0)
            {
                throw new InvalidOperationException("openFilesCount > 0");
            }

            var closed = Interlocked.Exchange(ref _closed, 1);

            if (closed == 0)
            {
                _orderContainer.Order = _operationContainer.LastOrder;

                _closeAction();
            }
        }


    }
}