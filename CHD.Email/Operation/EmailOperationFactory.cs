using System;
using System.Collections.Generic;
using System.IO;
using CHD.Common.Logger;
using CHD.Email.ServiceCode;
using CHD.Email.Token;
using CHD.FileSystem.FileWrapper;
using CHD.Graveyard;
using CHD.Graveyard.Operation;
using MailKit;
using MimeKit;

namespace CHD.Email.Operation
{
    public class EmailOperationFactory : IOperationFactory
    {
        private readonly EmailSettings _settings;
        private readonly IDisorderLogger _logger;

        public EmailOperationFactory(
            EmailSettings settings,
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

            using (var client = new ImapClientEx(_settings, _logger))
            {
                foreach (var summary in client.Scan())
                {
                    var uid = summary.UniqueId;
                    var subject = summary.Envelope.Subject;

                    var operation = new EmailOperation(
                        _settings,
                        uid,
                        subject,
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
            var operation = EmailOperation.CreateOperation(
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
            var operation = EmailOperation.CreateOperation(
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
            var operation = EmailOperation.CreateOperation(
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
            var operation = EmailOperation.CreateOperation(
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