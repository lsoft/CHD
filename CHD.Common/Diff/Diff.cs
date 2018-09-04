using System;
using System.Collections.Generic;
using CHD.Common.Operation;
using CHD.Common.OperationLog;

namespace CHD.Common.Diff
{
    public sealed class Diff : IDiff
    {
        private readonly IOperationLog _localLog;
        private readonly IOperationLog _remoteLog;

        public bool IsEmpty
        {
            get;
            private set;
        }

        public IOperationLog LocalLog
        {
            get
            {
                return _localLog;
            }
        }

        public IOperationLog RemoteLog
        {
            get
            {
                return _remoteLog;
            }
        }

        public Diff(
            IOperationLog localLog,
            IOperationLog remoteLog
            )
        {
            if (localLog == null)
            {
                throw new ArgumentNullException("localLog");
            }
            if (remoteLog == null)
            {
                throw new ArgumentNullException("remoteLog");
            }

            _localLog = localLog;
            _remoteLog = remoteLog;

            IsEmpty = (localLog.Count + remoteLog.Count) == 0;
        }

        public void Dump(IDisorderLogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            logger.LogMessage("Diff constructed:");

            logger.LogFormattedMessage(
                "Is empty: {0}", 
                IsEmpty
                );

            if (IsEmpty)
            {
                return;
            }

            _localLog.Dump(
                "Local",
                logger
                );
            _remoteLog.Dump(
                "Remote",
                logger
                );
        }

        public static IDiff Empty
        {
            get;
            private set;
        }

        static Diff()
        {
            Empty = new Diff(
                OperationLog.OperationLog.Empty,
                OperationLog.OperationLog.Empty
                );
        }

    }
}