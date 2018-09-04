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
        ///     Управляющий код поддержки службой уведомления о системном выключении
        /// </summary>
        private const int SERVICE_ACCEPT_PRESHUTDOWN = 0x100;

        /// <summary>
        ///     Управляющий код уведомления о системном выключении
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
            
            // подписываемся на команду SERVICE_CONTROL_PRESHUTDOWN
            //это необходимо чтобы винда при ребуте не убивала (выстрелом в голову) эту службу 
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
        ///     Запуск службы
        /// </summary>
        protected override void OnStart(string[] args)
        {
            // запуск в отладочном режиме
            if (args.Any(arg => arg == "-debug"))
            {
                RequestAdditionalTime((int) new TimeSpan(0, 60, 0).TotalMilliseconds);
                Debugger.Launch();
            }

            PostMessageToEventLogSafely(
                string.Format("Служба {0} выполняет запуск", RealServiceName)
                );

            // Запрос долгого таймаута для запуска приложения
            var startTimeout = new TimeSpan(0, 3, 0); //Тайм-аут = 3 мин

            this.RequestAdditionalTime((int) startTimeout.TotalMilliseconds);

            try
            {
                //new Thread(RunMessagePump).Start();


                //Создать поток
                _clientStarter.Start();

                PostMessageToEventLogSafely(
                    string.Format("Служба {0} успешно запущена", RealServiceName)
                    );
            }
            catch (Exception excp)
            {
                PostMessageToEventLogSafely(
                    string.Format(
                        "В процессе работы службы {0} произошла ошибка. Служба будет аварийно остановлена.\r\nОшибка: {1}\r\nСтек:{2}",
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
        ///     Остановка службы
        /// </summary>
        protected override void OnStop()
        {
            PostMessageToEventLogSafely(
                string.Format("Служба {0} выполняет останов", RealServiceName)
                );

            //Application.Exit();

            //Ждать корректного завершения работы потока в течении некоторого таймаута
            var requestAdditionalTimeout = (int) new TimeSpan(0, 1, 0).TotalMilliseconds; //Тайм-аут = 1 минута
            var waitTerminationTimeout = (int) new TimeSpan(0, 0, 30).TotalMilliseconds;
            var maxTerminationTimeout = new TimeSpan(0, 5, 0);
            
            RequestAdditionalTime(requestAdditionalTimeout);

            //стоппим в фоновом потоке
            var t = new Thread(
                () =>
                {
                    _clientStarter.SyncStop();
                });
            t.Start();

            // здесь будем ждать до тех пор пока не завершим работу либо пройдет 5 минут
            var runCycle = true;
            var stopTime = DateTime.Now;
            while (runCycle)
            {
                if (t.Join(waitTerminationTimeout))
                {
                    // Если поток завершен да, завершить работу метода и вывести сообщение в протокол
                    PostMessageToEventLogSafely(
                        string.Format("Служба {0} успешно остановлена", RealServiceName)
                        );

                    runCycle = false;
                }
                else
                {
                    if (DateTime.Now - stopTime < maxTerminationTimeout)
                    {
                        // Если еще не прошли 5 минут, то запрашиваем еще времени

                        RequestAdditionalTime(requestAdditionalTimeout);
                    }
                    else
                    {
                        // Максимальное время ожидания прошло, принудительно прервать работу потока
                        PostMessageToEventLogSafely(
                            string.Format(
                                "Рабочий поток службы {0} не завершился штатно в течении положенного тайм-аута. Служба будет остановлена принудительно.",
                                RealServiceName),
                            EventLogEntryType.Error);

                        Process.GetCurrentProcess().Kill();
                        Process.GetCurrentProcess().WaitForExit();
                    }
                }
            }
        }


        /// <summary>
        ///     Обработка нестандартных команд службы.
        ///     В данном случае обрабатываем SERVICE_CONTROL_PRESHUTDOWN - более продвинутый вариант
        ///     системного уведомления о выключении системы, которое дает больше времени на остановку службы.
        /// </summary>
        protected override void OnCustomCommand(int command)
        {
            if (command == SERVICE_CONTROL_PRESHUTDOWN)
            {
                PostMessageToEventLogSafely(
                    string.Format("Служба {0} получила предварительное уведомление о системном выключении.", RealServiceName)
                    );

                _clientStarter.SetWindowsIsShuttingDown();

                Stop();
            }
        }

        /// <summary>
        ///     Если поймаем сообщение о выключении системы, останавливаемся
        /// </summary>
        protected override void OnShutdown()
        {
            PostMessageToEventLogSafely(
                string.Format("Служба {0} получила уведомление о системном выключении.", RealServiceName)
                );

            Stop();
        }

        #endregion
    }
}