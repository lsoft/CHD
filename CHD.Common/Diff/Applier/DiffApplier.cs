//using CHD.Common.KeyValueContainer.SyncedVersion;
using System;
using System.Collections.Generic;
using CHD.Common.Breaker;
using CHD.Common.FileSystem;
using CHD.Common.Operation.Applier;
using CHD.Common.Operation.Applier.Factory;
using CHD.Common.Operation.Fixer;
using CHD.Common.Sync.Report;


namespace CHD.Common.Diff.Applier
{
    public sealed class DiffApplier : IDiffApplier
    {
        private const string RemoteBreakMessage = "Break during remote operation";
        private const string LocalBreakMessage = "Break during local operation";

        private readonly IOperationApplierFactory _operationApplierFactory;
        private readonly IReadBreaker _breaker;
        private readonly IDisorderLogger _logger;


        private readonly List<IOperationFixer> _localFixers = new List<IOperationFixer>();
        private readonly List<IOperationFixer> _remoteFixers = new List<IOperationFixer>();

        private FileSystemSyncReport _localReport;
        private FileSystemSyncReport _remoteReport;

        public DiffApplier(
            IOperationApplierFactory operationApplierFactory,
            IReadBreaker breaker,
            IDisorderLogger logger
            )
        {
            if (operationApplierFactory == null)
            {
                throw new ArgumentNullException("operationApplierFactory");
            }
            if (breaker == null)
            {
                throw new ArgumentNullException("breaker");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _operationApplierFactory = operationApplierFactory;
            _breaker = breaker;
            _logger = logger;
        }

        public void Apply(
            IFileSystem local,
            IFileSystem remote,
            IDiff diff
            )
        {
            if (local == null)
            {
                throw new ArgumentNullException("local");
            }
            if (remote == null)
            {
                throw new ArgumentNullException("remote");
            }
            if (diff == null)
            {
                throw new ArgumentNullException("diff");
            }

            _logger.LogMessage("Before applying");

            //preparation
            _localReport = new FileSystemSyncReport();
            _remoteReport = new FileSystemSyncReport();
            _localFixers.Clear();
            _remoteFixers.Clear();

            //do sync local -> remote
            try
            {
                DoLocalToRemote(
                    local,
                    remote,
                    diff
                    );

                //do sync local <- remote
                try
                {
                    DoRemoteToLocal(
                        local,
                        remote,
                        diff
                        );

                    _logger.LogMessage("After applying");

                    //looks like everything is ok
                }
                catch
                {
                    ClearLocalFixers();
                    throw;
                }
            }
            catch
            {
                ClearRemoteFixers();
                throw;
            }

        }

        public SyncReport Commit()
        {
            //looks like everything is ok

            _logger.LogMessage("Before commiting");

            //fix the changes
            CommitLocalFixers();
            CommitRemoteFixers();

            _logger.LogMessage("After commiting");

            var result = new SyncReport(
                SyncResultEnum.Completed,
                _localReport,
                _remoteReport
                );

            _logger.LogMessage("Sync completed");

            return
                result;
        }

        public void Revert()
        {
            ClearLocalFixers();

            ClearRemoteFixers();
        }

        private void DoRemoteToLocal(
            IFileSystem local,
            IFileSystem remote,
            IDiff diff
            )
        {
            _logger.LogMessage("Sync remote to local");

            foreach (var lo in diff.LocalLog.Operations)
            {
                _breaker.RaiseExceptionIfBreak(LocalBreakMessage);

                var fixer = lo.Apply(
                    remote.Copier,
                    _operationApplierFactory.Create(local)
                    );

                _localFixers.Add(fixer);

                lo.Accept(_localReport);
            }
        }

        private void DoLocalToRemote(
            IFileSystem local,
            IFileSystem remote,
            IDiff diff
            )
        {
            _logger.LogMessage("Sync local to remote");

            foreach (var ro in diff.RemoteLog.Operations)
            {
                _breaker.RaiseExceptionIfBreak(RemoteBreakMessage);

                var fixer = ro.Apply(
                    local.Copier,
                    _operationApplierFactory.Create(remote)
                    );

                _remoteFixers.Add(fixer);

                ro.Accept(_remoteReport);
            }
        }

        private void CommitRemoteFixers()
        {
            _remoteFixers.ForEach(j => j.SafelyCommit());
            _remoteFixers.Clear();
        }

        private void CommitLocalFixers()
        {
            _localFixers.ForEach(j => j.SafelyCommit());
            _localFixers.Clear();
        }

        private void ClearRemoteFixers()
        {
            _remoteFixers.Reverse();
            _remoteFixers.ForEach(j => j.SafelyRevert());
            _remoteFixers.Clear();
        }

        private void ClearLocalFixers()
        {
            _localFixers.Reverse();
            _localFixers.ForEach(j => j.SafelyRevert());
            _localFixers.Clear();
        }

    }
}
