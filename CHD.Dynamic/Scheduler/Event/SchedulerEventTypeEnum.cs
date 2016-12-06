namespace CHD.Dynamic.Scheduler.Event
{
    public enum SchedulerEventTypeEnum
    {
        /// <summary>
        /// ����������� ���������
        /// </summary>
        Started,

        /// <summary>
        /// ����������� �������� ���������������
        /// </summary>
        Stopping,

        /// <summary>
        /// ����������� ����������
        /// </summary>
        Stopped,

        /// <summary>
        /// ���������� ������ ������� ������
        /// </summary>
        TaskRaisedException,

        /// <summary>
        /// � ������������ ��� �����
        /// </summary>
        NoTask,

        /// <summary>
        /// ������ ���������� ������
        /// </summary>
        TaskBeginExecution,

        /// <summary>
        /// ����� ���������� ������
        /// </summary>
        TaskEndExecution,

        /// <summary>
        /// ����������� ������ ������ ��������, ����� ������� �� ����� ����������
        /// </summary>
        CriticalException,

        /// <summary>
        /// ������ �� ���� ��������� �� ���������� ��������
        /// </summary>
        TaskUncomplete
    }
}