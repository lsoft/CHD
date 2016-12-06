using System;
using System.Collections.Generic;

namespace CHD.Graveyard.Operation
{
    public interface IOperationContainer
    {
        long LastOrder
        {
            get;
        }

        void Add(
            IOperation operation
            );

        void Cleanup(
            );

        bool ContainsTransaction(
            Guid transactionGuid
            );

        List<IOperation> GetOperationsSince(
            long lastOrder
            );
    }
}