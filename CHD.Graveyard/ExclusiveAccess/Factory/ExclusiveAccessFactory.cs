using System;
using CHD.Common.KeyValueContainer.Order;
using CHD.Common.Logger;
using CHD.Graveyard.Operation;

namespace CHD.Graveyard.ExclusiveAccess.Factory
{
    public class ExclusiveAccessFactory : IExclusiveAccessFactory
    {
        private readonly IOperationContainerFactory _operationContainerFactory;
        private readonly IOperationFactory _operationFactory;
        private readonly IOrderContainer _orderContainer;
        private readonly IDisorderLogger _logger;

        public ExclusiveAccessFactory(
            IOperationContainerFactory operationContainerFactory,
            IOperationFactory operationFactory,
            IOrderContainer orderContainer,
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
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _operationContainerFactory = operationContainerFactory;
            _operationFactory = operationFactory;
            _orderContainer = orderContainer;
            _logger = logger;
        }

        public IExclusiveAccess GetExclusiveAccess(Action closeAction)
        {
            return 
                new ExclusiveAccess(
                    _operationContainerFactory,
                    _operationFactory,
                    _orderContainer,
                    closeAction,
                    _logger
                    );
        }
    }
}