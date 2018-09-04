using System;
using System.Threading;
using CHD.Common;
using CHD.Common.Sync.Factory;
using CHD.Common.Sync.Provider;

namespace CHD.Service.Runner
{
    public sealed class ScheduledScannerRunner : IDisposable, IScheduledScannerRunner
    {
        private const long NotStarted = 0L;
        private const long Started = 1L;
        private const long Stopped = 2L;

        private readonly int _timeoutMsec;
        private readonly ISynchronizerProvider _synchronizerProvider;
        private readonly IDisorderLogger _logger;

        private long _status = NotStarted;

        private readonly ManualResetEvent _stopSignal = new ManualResetEvent(false);
        private Thread _workThread;

        public ScheduledScannerRunner(
            int timeoutMsec,
            ISynchronizerProvider synchronizerProvider,
            IDisorderLogger logger
            )
        {
            if (synchronizerProvider == null)
            {
                throw new ArgumentNullException("synchronizerProvider");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _timeoutMsec = timeoutMsec;
            _synchronizerProvider = synchronizerProvider;
            _logger = logger;
        }

        public void AsyncStart()
        {
            if (Interlocked.CompareExchange(ref _status, Started, NotStarted) == NotStarted)
            {
                _workThread = new Thread(WorkThread);
                _workThread.Start();
            }
        }

        public void SyncStop()
        {
            if (Interlocked.Exchange(ref _status, Stopped) != Stopped)
            {
                DoStop();
            }
        }

        public void Dispose()
        {
            SyncStop();
        }

        private void WorkThread()
        {
            while (true)
            {
                //do syncing!
                var sync = _synchronizerProvider.CreateSynchronizer(
                    );

                try
                {
                    var syncReport = sync.Sync();
                }
                catch (Exception excp)
                {
                    _logger.LogException(excp);
                }

                //wait for timeout or stop signal
                var signalReceived = _stopSignal.WaitOne(_timeoutMsec);
                if (signalReceived)
                {
                    return;
                }
            }
        }

        private void DoStop()
        {
            SafelyStopWork();

            _stopSignal.Dispose();
        }

        private void SafelyStopWork()
        {
            _stopSignal.Set();

            var workThread = Interlocked.Exchange(ref _workThread, null);
            if (workThread != null)
            {
                workThread.Join();
            }
        }
    }
}
