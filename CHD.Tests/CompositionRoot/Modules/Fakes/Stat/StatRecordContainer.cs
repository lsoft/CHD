using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CHD.Common;
using PerformanceTelemetry.Record;

namespace CHD.Tests.CompositionRoot.Modules.Fakes.Stat
{
    public sealed class StatRecordContainer
    {
        private readonly ConcurrentDictionary<string, StatRecord> _records;

        public StatRecordContainer()
        {
            _records = new ConcurrentDictionary<string, StatRecord>();
        }

        public void AddRecord(
            IPerformanceRecordData record
            )
        {
            if (record == null)
            {
                throw new ArgumentNullException("record");
            }

            ProcessRecord(record);
        }

        public List<StatRecord> GetRecords()
        {
            return
                _records.Values.ToList();
        }

        public void Log(IDisorderLogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            var records = this.GetRecords();

            var total = TimeSpan.Zero;

            foreach (var record in records.OrderByDescending(j => j.Total))
            {
                total = total.Add(record.Total);

                record.Log(logger);
            }

            logger.LogFormattedMessage(
                "Total: {0}",
                total
                );
        }

        private void ProcessRecord(IPerformanceRecordData record)
        {
            var key = KeyProvider(record);

            var statRecord = _records.GetOrAdd(
                key,
                k => new StatRecord(k)
                );

            statRecord.AddRecord(
                record.TimeInterval
                );

            foreach (var cr in record.GetChildren())
            {
                ProcessRecord(cr);
            }

        }

        private string KeyProvider(IPerformanceRecordData record)
        {
            if (record == null)
            {
                throw new ArgumentNullException("record");
            }

            var result = string.Format(
                "{0}.{1}",
                record.ClassName,
                record.MethodName
                );

            return
                result;
        }
    }
}