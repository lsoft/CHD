using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace CHD.Service.WindowsService
{
    internal sealed class ServiceMode : ClientServiceIsolator
    {
        private readonly Arguments _arguments;
        private readonly ClientStarter _clientStarter;

        /// <summary>
        ///     ����������� ��� ��������� ������� ����������� � ��������� ����������
        /// </summary>
        private const int SERVICE_ACCEPT_PRESHUTDOWN = 0x100;

        /// <summary>
        ///     ����������� ��� ����������� � ��������� ����������
        /// </summary>
        private const int SERVICE_CONTROL_PRESHUTDOWN = 0x0F;

        /// <summary>
        ///      The service is notified when system shutdown occurs. 
        /// </summary>
        private const int SERVICE_ACCEPT_SHUTDOWN = 0x00000004;

        public ServiceMode(Arguments arguments)
        {
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }

            _arguments = arguments;
            
            // ������������� �� ������� SERVICE_CONTROL_PRESHUTDOWN
            //��� ���������� ����� ����� ��� ������ �� ������� (��������� � ������) ��� ������ 
            var serviceBaseType = typeof(ServiceBase);
            var acceptCommandsFieldInfo = serviceBaseType.GetField(
                "acceptedCommands",
                BindingFlags.Instance | BindingFlags.NonPublic
                );

            if (acceptCommandsFieldInfo != null)
            {
                var value = (int)acceptCommandsFieldInfo.GetValue(this);
                acceptCommandsFieldInfo.SetValue(this, value | SERVICE_ACCEPT_PRESHUTDOWN);
            }

            _clientStarter = new ClientStarter(
                arguments
                );
        }

        #region overrides On- functions

        /// <summary>
        ///     ������ ������
        /// </summary>
        protected override void OnStart(string[] args)
        {
            // ������ � ���������� ������
            if (args.Any(arg => arg == "-debug"))
            {
                RequestAdditionalTime((int) new TimeSpan(0, 60, 0).TotalMilliseconds);
                Debugger.Launch();
            }

            PostMessageToEventLogSafely(
                string.Format("������ {0} ��������� ������", RealServiceName)
                );

            // ������ ������� �������� ��� ������� ����������
            var startTimeout = new TimeSpan(0, 3, 0); //����-��� = 3 ���

            this.RequestAdditionalTime((int) startTimeout.TotalMilliseconds);

            try
            {
                //new Thread(RunMessagePump).Start();


                //������� �����
                _clientStarter.Start();

                PostMessageToEventLogSafely(
                    string.Format("������ {0} ������� ��������", RealServiceName)
                    );
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

        //void RunMessagePump()
        //{
        //    EventLog.WriteEntry("SimpleService.MessagePump", "Starting SimpleService Message Pump");
        //    Application.Run(new HiddenForm());
        //}


        /// <summary>
        ///     ��������� ������
        /// </summary>
        protected override void OnStop()
        {
            PostMessageToEventLogSafely(
                string.Format("������ {0} ��������� �������", RealServiceName)
                );

            //Application.Exit();

            //����� ����������� ���������� ������ ������ � ������� ���������� ��������
            var requestAdditionalTimeout = (int) new TimeSpan(0, 1, 0).TotalMilliseconds; //����-��� = 1 ������
            var waitTerminationTimeout = (int) new TimeSpan(0, 0, 30).TotalMilliseconds;
            var maxTerminationTimeout = new TimeSpan(0, 5, 0);
            
            RequestAdditionalTime(requestAdditionalTimeout);

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
        ///     ��������� ������������� ������ ������.
        ///     � ������ ������ ������������ SERVICE_CONTROL_PRESHUTDOWN - ����� ����������� �������
        ///     ���������� ����������� � ���������� �������, ������� ���� ������ ������� �� ��������� ������.
        /// </summary>
        protected override void OnCustomCommand(int command)
        {
            if (command == SERVICE_CONTROL_PRESHUTDOWN)
            {
                PostMessageToEventLogSafely(
                    string.Format("������ {0} �������� ��������������� ����������� � ��������� ����������.", RealServiceName)
                    );

                _clientStarter.SetWindowsIsShuttingDown();

                Stop();
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

            Stop();
        }

        #endregion
    }
}