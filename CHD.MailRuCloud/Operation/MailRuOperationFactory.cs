using System;
using System.Collections.Generic;
using CHD.Common.Logger;
using CHD.FileSystem.FileWrapper;
using CHD.Graveyard;
using CHD.Graveyard.Operation;
using CHD.MailRuCloud.ServiceCode;

namespace CHD.MailRuCloud.Operation
{
    public class MailRuOperationFactory : IOperationFactory
    {
        private readonly MailRuSettings _settings;
        private readonly IDisorderLogger _logger;

        public MailRuOperationFactory(
            MailRuSettings settings,
            IDisorderLogger logger
            )
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _settings = settings;
            _logger = logger;
        }

        public List<IOperation> GetAllOperations()
        {
            var result = new List<IOperation>();

            using (var client = new MailRuClientEx(_settings, _logger))
            {
                foreach (var fileNameContainer in client.Scan())
                {
                    var operation = new MailRuOperation(
                        _settings,
                        fileNameContainer,
                        _logger
                        );

                    result.Add(operation);
                }
            }

            return result;
        }

        public IOperation CreateBlockDataOperation(
            Guid transactionGuid, 
            long nextOrder, 
            IFileWrapper fileWrapper, 
            byte[] data
            )
        {
            var operation = MailRuOperation.CreateOperation(
                _logger,
                _settings,
                nextOrder,
                transactionGuid,
                GraveyardOperationTypeEnum.BlockData, 
                fileWrapper,
                data
                );

            return
                operation;
        }

        public IOperation CreateOpenFileOperation(
            Guid transactionGuid, 
            long nextOrder, 
            IFileWrapper fileWrapper
            )
        {
            var operation = MailRuOperation.CreateOperation(
                _logger,
                _settings,
                nextOrder,
                transactionGuid,
                GraveyardOperationTypeEnum.OpenFile,
                fileWrapper,
                null
                );

            return
                operation;
        }

        public IOperation CreateCloseFileOperation(
            Guid transactionGuid, 
            long nextOrder, 
            IFileWrapper fileWrapper
            )
        {
            var operation = MailRuOperation.CreateOperation(
                _logger,
                _settings,
                nextOrder,
                transactionGuid,
                GraveyardOperationTypeEnum.CloseFile,
                fileWrapper,
                null
                );

            return
                operation;
        }

        public IOperation CreateDeleteFileOperation(
            Guid transactionGuid,
            long nextOrder, 
            IFileWrapper fileWrapper
            )
        {
            var operation = MailRuOperation.CreateOperation(
                _logger,
                _settings,
                nextOrder,
                transactionGuid,
                GraveyardOperationTypeEnum.DeleteFile,
                fileWrapper
                );

            return
                operation;
        }
    }
}