using System.Collections.Generic;
using CHD.Common.Operation;
using CHD.Common.OperationLog;

namespace CHD.Common.Diff.Constructor
{
    public interface IConflictSearcher
    {
        IReadOnlyList<OperationPair> GetConflicts(
            IOperationLog localLog,
            IOperationLog remoteLog
            );
    }
}