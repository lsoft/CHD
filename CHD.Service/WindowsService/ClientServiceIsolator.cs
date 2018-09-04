using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace CHD.Service.WindowsService
{
    internal class ClientServiceIsolator : ServiceBase
    {
        public const string ServerServiceName = "CHD Service";
        public const string ServerDisplayName = "CHD Service";

        protected string RealServiceName
        {
            get
            {
                return
                    ServerServiceName;
            }
        }


        protected void PostMessageToEventLogSafely(
            string message,
            EventLogEntryType type = EventLogEntryType.Information
            )
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            try
            {
                EventLog.WriteEntry(
                    RealServiceName,
                    message,
                    type);
            }
            catch
            {
                //��������� ����������, ��� ��� ��� ����� ��������� ��� ������ EventLog.WriteEntry
                //����, ��������, ����� ����������� (��������� ����������������)
            }
        }

    }
}