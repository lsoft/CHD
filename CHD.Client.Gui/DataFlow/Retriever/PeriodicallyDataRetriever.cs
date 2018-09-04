using System;
using System.Linq;
using System.Threading;
using CHD.Client.Gui.Wcf;
using CHD.Common;

namespace CHD.Client.Gui.DataFlow.Retriever
{
    public class PeriodicallyDataRetriever : IDisposable, IDataRetriever
    {
        private const long NotStarted = 0L;
        private const long Started = 1L;
        private const long Stopped = 2L;

        private readonly IDataContainer _dataContainer;
        private readonly IDataChannelFactory _dataChannelFactory;
        private readonly IDisorderLogger _logger;

        private readonly ManualResetEvent _stopSignal = new ManualResetEvent(false);
        private Thread _workThread;

        private long _status = NotStarted;

        public PeriodicallyDataRetriever(
            IDataContainer dataContainer,
            IDataChannelFactory dataChannelFactory,
            IDisorderLogger logger
            )
        {
            if (dataContainer == null)
            {
                throw new ArgumentNullException("dataContainer");
            }
            if (dataChannelFactory == null)
            {
                throw new ArgumentNullException("dataChannelFactory");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _dataContainer = dataContainer;
            _dataChannelFactory = dataChannelFactory;
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
            var timeout = 1000;

            while (true)
            {
                try
                {
                    using (var dataChannel = _dataChannelFactory.OpenChannel())
                    {
                        var commonData = dataChannel.GetCommonInfo();
                        var dataChanged = _dataContainer.Set(commonData);

                        var syncData = dataChannel.GetSyncInfo();
                        dataChanged |= _dataContainer.Set(syncData);

                        var journalData = dataChannel.GetJournalInfo(_dataContainer.JournalData.Last);
                        dataChanged |= _dataContainer.Add(journalData);

                        if (dataChanged || syncData.IsInProgress)
                        {
                            timeout = 1000; //wait for 1 second if changes exists or sync is in progress
                        }
                        else
                        {
                            timeout = 10000; //wait for 10 seconds if changes does not exists and no sync exists
                        }
                    }
                }
                catch (Exception excp)
                {
                    _logger.LogException(excp);

                    timeout = 5000; //wait for 5 seconds in case of exception (probably no channel established)
                }

                //wait for timeout or stop signal
                var signalReceived = _stopSignal.WaitOne(timeout);
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