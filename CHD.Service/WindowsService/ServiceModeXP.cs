using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace CHD.Service.WindowsService
{
    internal sealed class ServiceModeXP : ClientServiceIsolator
    {
        private readonly Arguments _arguments;
        private readonly ClientStarter _clientStarter;

        public ServiceModeXP(Arguments arguments)
        {
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }

            _arguments = arguments;
            
            CanShutdown = true;

            _clientStarter = new ClientStarter(
                arguments
                );

            PostMessageToEventLogSafely(
                string.Format("�������� XP-������ {0} ������", RealServiceName)
                );

        }

        #region overrides On- functions

        /// <summary>
        ///     ������ ������
        /// </summary>
        protected override void OnStart(string[] args)
        {
            PostMessageToEventLogSafely(
                string.Format("������ {0} ��������� ������", RealServiceName)
                );

            // ������ � ���������� ������
            if (args.Any(arg => arg == "-debug"))
            {
                RequestAdditionalTime((int) new TimeSpan(0, 60, 0).TotalMilliseconds);
                Debugger.Launch();
            }

            // ������ ������� �������� ��� ������� ����������
            var startTimeout = new TimeSpan(0, 3, 0); //����-��� = 3 ���

            this.RequestAdditionalTime((int) startTimeout.TotalMilliseconds);

            try
            {
                //������� �����
                _clientStarter.Start();

                PostMessageToEventLogSafely(
                    string.Format("������ {0} ������� ��������", RealServiceName));
            }
            catch (Exception excp)
            {
                PostMessageToEventLogSafely(
                    string.Format(
                        "� �������� ������ ������ {0} ��������� ������. ������ ����� �������� �����������.\r\n������: {1}\r\n����:{2}",
                        RealServiceName,
                        excp.Message,
                        excp.StackTrace),
                    EventLogEntryType.Error);

                ExitCode = 1359;

                Stop();
            }
        }


        /// <summary>
        ///     ��������� ������
        /// </summary>
        protected override void OnStop()
        {
            PostMessageToEventLogSafely(
                string.Format("������ {0} ��������� �������", RealServiceName)
                );

            //����� ����������� ���������� ������ ������ � ������� ���������� ��������
            var requestAdditionalTimeout = (int) new TimeSpan(0, 1, 0).TotalMilliseconds; //����-��� = 1 ������
            var waitTerminationTimeout = (int) new TimeSpan(0, 0, 30).TotalMilliseconds;
            var maxTerminationTimeout = new TimeSpan(0, 5, 0);
            
            //RequestAdditionalTime(requestAdditionalTimeout);

            //������� � ������� ������
            var t = new Thread(
                () =>
                {
                    _clientStarter.SyncStop();
                });
            t.Start();

            // ����� ����� ����� �� ��� ��� ���� �� �������� ������ ���� ������� 5 �����
            var runCycle = true;
            var stopTime = DateTime.Now;
            while (runCycle)
            {
                if (t.Join(waitTerminationTimeout))
                {
                    // ���� ����� �������� ��, ��������� ������ ������ � ������� ��������� � ��������
                    PostMessageToEventLogSafely(
                        string.Format("������ {0} ������� �����������", RealServiceName)
                        );

                    runCycle = false;
                }
                else
                {
                    if (DateTime.Now - stopTime < maxTerminationTimeout)
                    {
                        // ���� ��� �� ������ 5 �����, �� ����������� ��� �������

                        RequestAdditionalTime(requestAdditionalTimeout);
                    }
                    else
                    {
                        // ������������ ����� �������� ������, ������������� �������� ������ ������
                        PostMessageToEventLogSafely(
                            string.Format(
                                "������� ����� ������ {0} �� ���������� ������ � ������� ����������� ����-����. ������ ����� ����������� �������������.",
                                RealServiceName),
                            EventLogEntryType.Error);

                        Process.GetCurrentProcess().Kill();
                        Process.GetCurrentProcess().WaitForExit();
                    }
                }
            }
        }


        /// <summary>
        ///     ���� ������� ��������� � ���������� �������, ���������������
        /// </summary>
        protected override void OnShutdown()
        {
            PostMessageToEventLogSafely(
                string.Format("������ {0} �������� ����������� � ��������� ����������.", RealServiceName)
                );

            _clientStarter.SetWindowsIsShuttingDown();

            OnStop();
        }

        #endregion
    }
}