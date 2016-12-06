using System;
using System.Collections.Generic;
using System.Linq;
using CHD.Common;

namespace CHD.Graveyard.Operation
{
    public class OperationContainer : IOperationContainer
    {
        private readonly List<IOperation> _operations;

        public long LastOrder
        {
            get;
            private set;
        }

        public OperationContainer(
            IOperationFactory operationFactory
            )
        {
            if (operationFactory == null)
            {
                throw new ArgumentNullException("operationFactory");
            }

            _operations = operationFactory.GetAllOperations();

            Cleanup();
        }

        public void Add(
            IOperation operation
            )
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            _operations.Add(operation);

            ReassignLastOrder(operation);
        }

        public void Cleanup()
        {
            //at first we should delete invalid (uncompleted) transactions
            RemoveUncompletedTransactions(
                );

            //after that we should delete obsolete transactions
            RemoveObsoleteTransactions(
                );

            ReassignLastOrder();
        }

        public bool ContainsTransaction(Guid transactionGuid)
        {
            return
                _operations.Any(j => j.TransactionGuid == transactionGuid);
        }

        public List<IOperation> GetOperationsSince(long lastOrder)
        {
            return
                _operations.FindAll(j => j.Order > lastOrder);
        }

        private void ReassignLastOrder(
            IOperation operation
            )
        {
            if (operation != null)
            {
                LastOrder = Math.Max(LastOrder, operation.Order);
            }
        }

        private void ReassignLastOrder()
        {
            if (_operations.Count > 0)
            {
                LastOrder = _operations.Max(j => j.Order);
            }
        }

        private void RemoveObsoleteTransactions(
            )
        {
            if (_operations.Count == 0)
            {
                return;
            }


            var groups =
                from o in _operations
                group o by o.FilePathSuffix.Key into go
                select
                    go.Select(j => j.TransactionGuid).Distinct()
                ;


            foreach (var g in groups)
            {
                var tguids = g.ToList();

                if (tguids.Count > 1)
                {
                    //2 or more transactions with same file found in the graveyard!

                    var olderTransactionGuid = (
                        from tg in tguids
                        let minop = _operations.Find(j => j.TransactionGuid == tg)
                        let minimalOrder = minop != null ? minop.Order : -1
                        orderby minimalOrder ascending
                        select tg
                        )
                        .First();

                    var operations2Delete = (
                        _operations
                            .Where(j => j.TransactionGuid == olderTransactionGuid)
                        ).ToList();

                    foreach (var op in operations2Delete)
                    {
                        op.Delete();
                        _operations.Remove(op);
                    }
                }
            }
        }

        private void RemoveUncompletedTransactions(
            )
        {
            if (_operations.Count == 0)
            {
                return;
            }

            var tids = (
                from o in _operations
                where o.OperationType.In(GraveyardOperationTypeEnum.OpenFile, GraveyardOperationTypeEnum.BlockData, GraveyardOperationTypeEnum.CloseFile)
                group o by o.TransactionGuid
                    into go
                    where go.All(j => j.OperationType != GraveyardOperationTypeEnum.CloseFile)
                    select go.Key
                ).Distinct().ToList();

            foreach (var tid in tids)
            {
                Guid tid1 = tid;

                var filtered0 =
                    _operations
                        .Where(j => j.TransactionGuid == tid1)
                        .ToLookup(j => j.OperationType, k => k)
                    ;

                //at first we should delete block data operations
                foreach (var op in filtered0[GraveyardOperationTypeEnum.BlockData])
                {
                    op.Delete();
                    _operations.Remove(op);
                }

                //at last we should delete open transaction operation
                foreach (var op in filtered0[GraveyardOperationTypeEnum.OpenFile])
                {
                    op.Delete();
                    _operations.Remove(op);
                }
            }
        }
    }
}
