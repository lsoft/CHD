using System;
using CHD.Dynamic.Scheduler.Event;
using CHD.Dynamic.Scheduler.Task;

namespace CHD.Dynamic.Scheduler
{
    /// <summary>
    /// ����������� �����
    /// </summary>
    public interface IScheduler
    {
        /// <summary>
        /// �������, ������������ ���������.
        /// ����������� ���� ������� �� ������ �������� ��������� �����,
        /// ��� ��� ������� ���������� �� ������� ������� ��������.
        /// ��������� ������� - ������ ��������� ������� � ������� ��������� � ���������� ����������,
        /// ����� �� ������� ��� �������, � ������� ��������� � ��������� ������.
        /// </summary>
        event SchedulerEventDelegate SchedulerEvent;

        /// <summary>
        /// ������� ���������� ������ � ��������
        /// </summary>
        int TaskCount
        {
            get;
        }

        /// <summary>
        /// ����� ��������.
        /// </summary>
        void Start(
            );


        /// <summary>
        /// ������� ��������
        /// </summary>
        void Stop(
            );

        /// <summary>
        /// �������� ���� � �������.
        /// </summary>
        void AddTask(
            ITask task
            );

        /// <summary>
        /// �������� ��������� ������.
        /// ���� ������ ��� ������ �����������, �� �� ���������� �� ����� ��������.
        /// </summary>
        /// <param name="taskGuid">���� ������</param>
        void CancelTask(
            Guid taskGuid
            );
    }
}