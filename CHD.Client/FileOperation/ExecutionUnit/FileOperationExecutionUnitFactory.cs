using System;
using CHD.Client.FileOperation.Container;
using CHD.Common.Logger;
using CHD.Graveyard.RemoteFileState;
using CHD.Pull.Components;
using CHD.Pull.Components.Factory;

namespace CHD.Client.FileOperation.ExecutionUnit
{
    internal class FileOperationExecutionUnitFactory : IExecutionUnitFactory
    {
        private readonly IExecutionUnitFactory _executionUnitFactory;
        private readonly IActualFileOperationsContainer _actualFileOperationsContainer;
        private readonly IDisorderLogger _logger;

        public FileOperationExecutionUnitFactory(
            IExecutionUnitFactory executionUnitFactory,
            IActualFileOperationsContainer actualFileOperationsContainer,
            IDisorderLogger logger
            )
        {
            if (executionUnitFactory == null)
            {
                throw new ArgumentNullException("executionUnitFactory");
            }
            if (actualFileOperationsContainer == null)
            {
                throw new ArgumentNullException("actualFileOperationsContainer");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _executionUnitFactory = executionUnitFactory;
            _actualFileOperationsContainer = actualFileOperationsContainer;
            _logger = logger;
        }

        public IExecutionUnit Create(
            IRemoteFileState fileState
            )
        {
            if (fileState == null)
            {
                throw new ArgumentNullException("fileState");
            }

            var eu = _executionUnitFactory.Create(
                fileState
                );

            var eeu = new FileOperationExecutionUnit(
                _actualFileOperationsContainer,
                eu,
                _logger
                );

            return
                eeu;
        }
    }
}