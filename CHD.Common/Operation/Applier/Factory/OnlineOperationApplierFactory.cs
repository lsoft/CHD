using System;
using CHD.Common.FileSystem;
using CHD.Common.OnlineStatus.Diff.Apply.Operation;

namespace CHD.Common.Operation.Applier.Factory
{
    public sealed class OnlineOperationApplierFactory : IOperationApplierFactory
    {
        private readonly IOperationApplierFactory _factory;
        private readonly IOperationOnlineStatus _onlineStatus;

        public OnlineOperationApplierFactory(
            IOperationApplierFactory factory,
            IOperationOnlineStatus onlineStatus
            )
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }
            if (onlineStatus == null)
            {
                throw new ArgumentNullException("onlineStatus");
            }
            _factory = factory;
            _onlineStatus = onlineStatus;
        }

        public IOperationApplier Create(IFileSystem fileSystem)
        {
            var inner = _factory.Create(fileSystem);

            var result = new OnlineOperationApplier(
                inner,
                _onlineStatus
                );

            return result;
        }
    }
}