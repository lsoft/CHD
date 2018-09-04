using System;
using System.Collections.Generic;
using System.Threading;
using CHD.Common;
using CHD.Common.Others;

namespace CHD.Tests.ExtendedContext.StatRecord
{
    public sealed class StatisticRecordRecord : IStatisticRecord, IStatisticRecordFactory
    {
        public string Header
        {
            get;
            private set;
        }

        public TimeSpan Elapsed
        {
            get;
            private set;
        }

        public List<StatisticRecordRecord> Statistics
        {
            get;
            private set;
        }

        private readonly int _prefixSpaceCount;
        private readonly IDisorderLogger _logger;
        
        private readonly PerformanceTimer _timer;

        private long _disposed = 0L;

        public StatisticRecordRecord(
            string header,
            IDisorderLogger logger
            ) : this(0, header, logger)
        {
            
        }

        private StatisticRecordRecord(
            int prefixSpaceCount,
            string header,
            IDisorderLogger logger
            )
        {
            if (header == null)
            {
                throw new ArgumentNullException("header");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _prefixSpaceCount = prefixSpaceCount;
            Header = header;
            _logger = logger;

            _timer = new PerformanceTimer();

            Statistics = new List<StatisticRecordRecord>();
        }

        public void Log()
        {
            Dispose();

            _logger.LogFormattedMessage(
                "{0}{1} taken: {2} seconds",
                new string(' ', _prefixSpaceCount),
                Header,
                Elapsed
                );

            foreach (var statistic in Statistics)
            {
                statistic.Log();
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1L) == 0L)
            {
                Elapsed = TimeSpan.FromSeconds(_timer.Seconds);
            }
        }

        public IStatisticRecord Create(string header)
        {
            var result = new StatisticRecordRecord(
                _prefixSpaceCount + 4,
                header,
                _logger
                );

            Statistics.Add(result);

            return
                result;
        }
    }
}
