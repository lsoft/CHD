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
                string.Format("Изолятор XP-службы {0} создан", RealServiceName)
                );

        }

        #region overrides On- functions

        /// <summary>
        ///     Запуск службы
        /// </summary>
        protected override void OnStart(string[] args)
        {
            PostMessageToEventLogSafely(
                string.Format("Служба {0} выполняет запуск", RealServiceName)
                );

            // запуск в отладочном режиме
            if (args.Any(arg => arg == "-debug"))
            {
                RequestAdditionalTime((int) new TimeSpan(0, 60, 0).TotalMilliseconds);
                Debugger.Launch();
            }

            // Запрос долгого таймаута для запуска приложения
            var startTimeout = new TimeSpan(0, 3, 0); //Тайм-аут = 3 мин

            this.RequestAdditionalTime((int) startTimeout.TotalMilliseconds);

            try
            {
                //Создать поток
                _clientStarter.Start();

                PostMessageToEventLogSafely(
                    string.Format("Служба {0} успешно запущена", RealServiceName));
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


        /// <summary>
        ///     Остановка службы
        /// </summary>
        protected override void OnStop()
        {
            PostMessageToEventLogSafely(
                string.Format("Служба {0} выполняет останов", RealServiceName)
                );

            //Ждать корректного завершения работы потока в течении некоторого таймаута
            var requestAdditionalTimeout = (int) new TimeSpan(0, 1, 0).TotalMilliseconds; //Тайм-аут = 1 минута
            var waitTerminationTimeout = (int) new TimeSpan(0, 0, 30).TotalMilliseconds;
            var maxTerminationTimeout = new TimeSpan(0, 5, 0);
            
            //RequestAdditionalTime(requestAdditionalTimeout);

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
        ///     Если поймаем сообщение о выключении системы, останавливаемся
        /// </summary>
        protected override void OnShutdown()
        {
            PostMessageToEventLogSafely(
                string.Format("Служба {0} получила уведомление о системном выключении.", RealServiceName)
                );

            _clientStarter.SetWindowsIsShuttingDown();

            OnStop();
        }

        #endregion
    }
}