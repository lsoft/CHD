using System;
using CHD.Common.Diff.Conflict;
using CHD.Common.FileSystem;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.Operation;
using CHD.Common.OperationLog;

namespace CHD.Common.Diff.Constructor
{
    public sealed class DiffConstructor : IDiffConstructor
    {
        private readonly IOperationLogGenerator _logGenerator;
        private readonly IDisorderLogger _logger;

        public DiffConstructor(
            IOperationLogGenerator logGenerator,
            IDisorderLogger logger
            )
        {
            if (logGenerator == null)
            {
                throw new ArgumentNullException("logGenerator");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _logGenerator = logGenerator;
            _logger = logger;
        }

        public IDiff BuildDiff(
            IFolder localActual,
            IFolder localLastSynced,
            IFolder remoteActual,
            out IConflictDescription conflictDescription
            )
        {
            if (localActual == null)
            {
                throw new ArgumentNullException("localActual");
            }
            if (localLastSynced == null)
            {
                throw new ArgumentNullException("localLastSynced");
            }
            if (remoteActual == null)
            {
                throw new ArgumentNullException("remoteActual");
            }

            _logger.LogMessage("Building diff...");

            //var lastSynced = local.LastStructure.RootFolder;
            //var actual = _scanner.Scan();

            var localLog = _logGenerator.Generate(
                localLastSynced,
                localActual
                );

            var remoteLog = _logGenerator.Generate(
                localLastSynced,
                remoteActual
                );


            if (localLog.IsEmpty && remoteLog.IsEmpty)
            {
                conflictDescription = ConflictDescription.Empty;

                return
                    Diff.Empty;
            }

            var cs = new ConflictSearcher();

            var conflicts = cs.GetConflicts(
                localLog,
                remoteLog
                );

            conflictDescription = new ConflictDescription(
                conflicts
                );

            var diff = new Diff(
                remoteLog,
                localLog
                );

            diff.Dump(_logger);

            return
                diff;
        }
    }
}