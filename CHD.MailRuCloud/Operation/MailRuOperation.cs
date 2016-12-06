using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHD.Common.Logger;
using CHD.FileSystem.Algebra;
using CHD.FileSystem.FileWrapper;
using CHD.Graveyard;
using CHD.Graveyard.Operation;
using CHD.MailRuCloud.ServiceCode;

namespace CHD.MailRuCloud.Operation
{
    [DebuggerDisplay("Order = {Order} {OperationType} {FilePathSuffix} MailGuid = {_fileNameContainer.MailGuid}")]
    public class MailRuOperation : IOperation
    {
        private readonly MailRuSettings _settings;
        private readonly MailRuOperationFileNameContainer _fileNameContainer;
        private readonly IDisorderLogger _logger;

        public long Order
        {
            get
            {
                return
                    _fileNameContainer.Order;
            }
        }

        public Guid TransactionGuid
        {
            get
            {
                return
                    _fileNameContainer.TransactionGuid;
            }
        }

        public GraveyardOperationTypeEnum OperationType
        {
            get
            {
                return
                    _fileNameContainer.OperationType;
            }
        }

        public Suffix FilePathSuffix
        {
            get
            {
                return
                    _fileNameContainer.FilePathSuffix;
            }
        }

        public MailRuOperation(
            MailRuSettings settings,
            MailRuOperationFileNameContainer fileNameContainer,
            IDisorderLogger logger
            )
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (fileNameContainer == null)
            {
                throw new ArgumentNullException("fileNameContainer");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _settings = settings;
            _fileNameContainer = fileNameContainer;
            _logger = logger;
        }

        internal static MailRuOperation CreateOperation(
            IDisorderLogger logger,
            MailRuSettings settings,
            long order,
            Guid transactionGuid,
            GraveyardOperationTypeEnum operationType,
            IFileWrapper fileWrapper,
            byte[] data = null
            )
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (fileWrapper == null)
            {
                throw new ArgumentNullException("fileWrapper");
            }

            var mailGuid = Guid.NewGuid();
            var fileName = MailRuFileNameComposer.ComposeFileName(
                order,
                transactionGuid,
                operationType,
                mailGuid,
                fileWrapper
                );
            var fileNameContainer = new MailRuOperationFileNameContainer(fileName);

            //append the email to Sent folder
            using (var client = new MailRuClientEx(settings, logger))
            {
                client.AppendMessage(
                    fileNameContainer,
                    data
                    );
            }

            var operation = new MailRuOperation(
                settings,
                fileNameContainer,
                logger
                );

            return
                operation;
        }

        public void WriteRemoteDataTo(
            Stream destination
            )
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            if (OperationType != GraveyardOperationTypeEnum.BlockData)
            {
                throw new InvalidOperationException("OperationType != GraveyardOperationTypeEnum.BlockData");
            }

            using (var client = new MailRuClientEx(_settings, _logger))
            {
                client.DecodeAttachmentTo(
                    _fileNameContainer.MailGuid,
                    destination
                    );
            }
        }

        public void Delete()
        {
            DoDelete();
        }

        private void DoDelete(
            )
        {
            using (var client = new MailRuClientEx(_settings, _logger))
            {
                client.DeleteMessage(_fileNameContainer.MailGuid);
            }
        }
    }
}
