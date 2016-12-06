using System;
using System.Collections.Generic;
using System.IO;
using CHD.Ambient;
using CHD.Client.Marker.Factory;
using CHD.Common.Logger;

namespace CHD.Client.Marker.History
{
    internal class RecordContainer : IRecordContainer
    {
        private const int StorePeriodInDays = 60;

        private readonly string _filepath;
        private readonly IEventMarkerFactory _eventMarkerFactory;
        private readonly IDisorderLogger _logger;

        private readonly List<Record> _list = new List<Record>();

        public RecordContainer(
            string filepath,
            IEventMarkerFactory eventMarkerFactory,
            IDisorderLogger logger
            )
        {
            if (filepath == null)
            {
                throw new ArgumentNullException("filepath");
            }
            if (eventMarkerFactory == null)
            {
                throw new ArgumentNullException("eventMarkerFactory");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _filepath = filepath;
            _eventMarkerFactory = eventMarkerFactory;
            _logger = logger;
        }


        public void Prepare()
        {
            var fi = new FileInfo(_filepath);

            if (fi.Directory != null)
            {
                var di = fi.Directory.FullName;
                if (!Directory.Exists(di))
                {
                    Directory.CreateDirectory(di);
                }
            }

            if (fi.Exists)
            {
                ReadFile();
            }
            else
            {
                CreateFile();
            }

            Cleanup();

            _eventMarkerFactory.MarkerStatusChangedEvent += OnMarkerStatusChanged;
        }

        private void OnMarkerStatusChanged(bool taken, Exception exception)
        {
            Record r;

            if (exception == null)
            {
                r = new Record(
                    AmbientContext.DateTimeProvider.GetCurrentTime(),
                    taken
                    );
            }
            else
            {
                r = new Record(
                    AmbientContext.DateTimeProvider.GetCurrentTime(),
                    taken,
                    string.Format(
                        "{0} --- {1}",
                        exception.Message,
                        exception.StackTrace
                        )
                    );
            }

            this.Add(r);
        }

        public event RecordContainerChangedDelegate StatusChangedEvent;

        public void Add(
            Record record
            )
        {
            if (record == null)
            {
                throw new ArgumentNullException("record");
            }

            _list.Add(record);

            AppendOne(record);

            OnStatusChanged(
                new List<Record>
                {
                    record
                }
            );
        }

        protected virtual void OnStatusChanged(List<Record> newRecords)
        {
            RecordContainerChangedDelegate handler = StatusChangedEvent;
            if (handler != null)
            {
                handler(newRecords);
            }
        }

        private void AppendOne(Record record)
        {
            if (record == null)
            {
                throw new ArgumentNullException("record");
            }

            var l = record.Serialize();

            File.AppendAllLines(
                _filepath,
                new [] { l }
                );
        }

        private void Cleanup()
        {
            var border = AmbientContext.DateTimeProvider.GetCurrentTime().AddDays(-StorePeriodInDays);

            var removed = _list.RemoveAll(j => j.Time < border);
            if (removed > 0)
            {
                SafelyRemoveFile();

                SaveAll();
            }
        }

        private void SaveAll()
        {
            string data;

            if (_list.Count > 0)
            {
                data = string.Join(
                    Environment.NewLine,
                    _list.ConvertAll(j => j.Serialize())
                    ) + Environment.NewLine;
            }
            else
            {
                data = string.Empty;
            }

            File.WriteAllText(_filepath, data);
        }

        private void SafelyRemoveFile()
        {
            if (File.Exists(_filepath))
            {
                try
                {
                    File.Delete(_filepath);
                }
                catch (Exception excp)
                {
                    _logger.LogException(excp);
                }
            }
        }

        private void CreateFile()
        {
            using (var fs = new FileStream(_filepath, FileMode.CreateNew, FileAccess.Write))
            {
                fs.Flush();
            }
        }

        private void ReadFile()
        {
            var lines = File.ReadAllLines(_filepath);

            foreach (var l in lines)
            {
                try
                {
                    var r = Record.Deserialize(l);
                    _list.Add(r);
                }
                catch (Exception excp)
                {
                    _logger.LogException(excp);
                }
            }

            OnStatusChanged(_list);
        }
    }
}