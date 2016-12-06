using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CHD.Common.Logger;
using CHD.FileSystem.Algebra;
using CHD.Graveyard.Operation;

namespace CHD.Graveyard.RemoteFileState
{
    public class RemoteFileState : IRemoteFileState
    {
        private readonly List<IOperation> _operations;
        private readonly IDisorderLogger _logger;

        public long Order
        {
            get;
            private set;
        }

        public Suffix FilePathSuffix
        {
            get;
            private set;
        }

        public bool ShouldBeDeleted
        {
            get;
            private set;
        }

        public RemoteFileState(
            List<IOperation> operations,
            IDisorderLogger logger
            )
        {
            if (operations == null)
            {
                throw new ArgumentNullException("operations");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            if (operations.Count == 0)
            {
                throw new ArgumentException("operations.Count == 0");
            }

            _operations = operations;
            _logger = logger;

            var firstOperation = operations.First();
            var lastOperation = operations.Last();

            Order = lastOperation.Order;
            FilePathSuffix = firstOperation.FilePathSuffix;
            ShouldBeDeleted = firstOperation.OperationType == GraveyardOperationTypeEnum.DeleteFile;
        }

        public void WriteTo(
            Stream destination,
            Action<int, int> progressChangeFunc = null
            )
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            if (ShouldBeDeleted)
            {
                throw new InvalidOperationException("ShouldBeDeleted is true");
            }

            progressChangeFunc = progressChangeFunc ?? ((a, b) => { });

            var dataops = (
                from o in _operations
                where o.OperationType == GraveyardOperationTypeEnum.BlockData
                orderby o.Order
                select o
                ).ToList();

            for(var cc = 0; cc < dataops.Count; cc++)
            {
                progressChangeFunc(cc, dataops.Count);

                _logger.LogFormattedMessage(
                    "Download progress for the file {0} {1}\\{2}",
                    this.FilePathSuffix,
                    cc,
                    dataops.Count
                    );

                var dop = dataops[cc];

                dop.WriteRemoteDataTo(destination);
            }

            progressChangeFunc(dataops.Count, dataops.Count);

            _logger.LogFormattedMessage(
                "Download completed for the file {0}",
                this.FilePathSuffix
                );
        }

    }
}