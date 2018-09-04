using System;
using CHD.Common.FileSystem;
using CHD.Common.OnlineStatus.Diff.Apply.Operation;
using CHD.Common.Operation.FileOperation;
using CHD.Common.Operation.Fixer;
using CHD.Common.Operation.FolderOperation;

namespace CHD.Common.Operation.Applier
{
    public sealed class OnlineOperationApplier : IOperationApplier
    {
        private readonly IOperationApplier _operationApplier;
        private readonly IOperationOnlineStatus _onlineStatus;

        public OnlineOperationApplier(
            IOperationApplier operationApplier,
            IOperationOnlineStatus onlineStatus
            )
        {
            if (operationApplier == null)
            {
                throw new ArgumentNullException("operationApplier");
            }
            if (onlineStatus == null)
            {
                throw new ArgumentNullException("onlineStatus");
            }
            _operationApplier = operationApplier;
            _onlineStatus = onlineStatus;
        }

        public IOperationFixer ApplyOperation(
            IFileSystemCopier sourceFileSystemCopier,
            IFileOperation operation
            )
        {
            _onlineStatus.Start(operation);
            try
            {
                var result = _operationApplier.ApplyOperation(
                    sourceFileSystemCopier,
                    operation
                    );

                _onlineStatus.EndWithSuccess(operation);

                return
                    result;
            }
            catch
            {
                _onlineStatus.EndWithErrors(operation);
                throw;
            }
        }

        public IOperationFixer ApplyOperation(
            IFileSystemCopier sourceFileSystemCopier,
            IFolderOperation operation
            )
        {
            _onlineStatus.Start(operation);
            try
            {
                var result = _operationApplier.ApplyOperation(
                    sourceFileSystemCopier,
                    operation
                    );

                _onlineStatus.EndWithSuccess(operation);

                return
                    result;
            }
            catch
            {
                _onlineStatus.EndWithErrors(operation);
                throw;
            }
        }
    }
}