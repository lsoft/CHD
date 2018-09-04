using System.Collections.Generic;
using CHD.Common.Operation;
using CHD.Common.OperationLog;

namespace CHD.Common.Diff
{
    public interface IDiff
    {
        bool IsEmpty
        {
            get;
        }

        /// <summary>
        /// операции, которые необходимо применить на local, чтобы было всё синхронизировано
        /// </summary>
        IOperationLog LocalLog
        {
            get;
        }

        /// <summary>
        /// операции, которые необходимо применить на remote, чтобы было всё синхронизировано
        /// </summary>
        IOperationLog RemoteLog
        {
            get;
        }
    }
}