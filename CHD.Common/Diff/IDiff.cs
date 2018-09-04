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
        /// ��������, ������� ���������� ��������� �� local, ����� ���� �� ����������������
        /// </summary>
        IOperationLog LocalLog
        {
            get;
        }

        /// <summary>
        /// ��������, ������� ���������� ��������� �� remote, ����� ���� �� ����������������
        /// </summary>
        IOperationLog RemoteLog
        {
            get;
        }
    }
}