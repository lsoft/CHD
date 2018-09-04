using System;
using System.Collections.Generic;
using System.Diagnostics;
using CHD.Common.Operation;
using CHD.Common.Others;

namespace CHD.Common.Diff.Conflict
{
    public sealed class ConflictDescription : IConflictDescription
    {
        private readonly IReadOnlyList<OperationPair> _conflictedOperations;

        public bool ConflictExists
        {
            get;
            private set;
        }

        public ConflictDescription()
        {
            ConflictExists = false;
        }

        public ConflictDescription(
            IReadOnlyList<OperationPair> conflictedOperations
            )
        {
            if (conflictedOperations == null)
            {
                throw new ArgumentNullException("conflictedOperations");
            }

            _conflictedOperations = conflictedOperations;

            ConflictExists = conflictedOperations.Count > 0;
        }

        public void RaiseExceptionIfConfictExists()
        {
            if (!ConflictExists)
            {
                return;
            }

            var s = _conflictedOperations.Join(
                Environment.NewLine,
                pair => string.Format("local {0} <-> remote {1}", pair.Local.HumanReadableDescription, pair.Remote.HumanReadableDescription)
                );

            throw new CHDException(
                string.Format(
                    "Conflict(s) has been found: {0}{1}",
                    Environment.NewLine,
                    s
                    ),
                CHDExceptionTypeEnum.OperationConfict
                );
        }

        public static ConflictDescription Empty
        {
            get;
            private set;
        }

        static ConflictDescription()
        {
            Empty = new ConflictDescription();
        }
    }
}