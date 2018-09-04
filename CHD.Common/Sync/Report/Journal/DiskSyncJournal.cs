using System;
using System.Collections.Generic;
using System.IO;
using CHD.Common.Serializer;

namespace CHD.Common.Sync.Report.Journal
{
    public sealed class DiskSyncJournal : ISyncJournal
    {
        private readonly string _filePath;
        private readonly int _storeDays;
        private readonly ISerializer _serializer;
        private readonly List<SyncReport> _list;

        public IReadOnlyList<SyncReport> Reports
        {
            get
            {
                return 
                    _list;
            }
        }

        public DiskSyncJournal(
            string filePath,
            int storeDays,
            ISerializer serializer
            )
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }
            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            _filePath = filePath;
            _storeDays = storeDays;
            _serializer = serializer;

            _list = ReadOrCreate();
            Cleanup();
        }

        public void Add(
            SyncReport report
            )
        {
            if (report == null)
            {
                throw new ArgumentNullException("report");
            }

            _list.Add(report);


            Cleanup();
            Save();
        }

        private void Save()
        {
            var fi = new FileInfo(_filePath);
            var newFilePath = Path.Combine(fi.DirectoryName, Guid.NewGuid().ToString());
            var backupFilePath = Path.Combine(fi.DirectoryName, Guid.NewGuid().ToString());

            using (var fs = new FileStream(newFilePath, FileMode.CreateNew, FileAccess.Write))
            {
                _serializer.Serialize(
                    _list,
                    fs
                    );
            }

            try
            {
                if (File.Exists(_filePath))
                {
                    File.Replace(
                        newFilePath,
                        _filePath,
                        backupFilePath
                        );
                }
                else
                {
                    File.Move(
                        newFilePath,
                        _filePath
                        );
                }
            }
            catch
            {
                if (File.Exists(backupFilePath))
                {
                    try
                    {
                        File.Delete(_filePath);
                        File.Move(backupFilePath, _filePath);
                    }
                    catch
                    {
                        //nothing to do
                    }
                }

                if (File.Exists(newFilePath))
                {
                    try
                    {
                        File.Delete(newFilePath);
                    }
                    catch
                    {
                        //nothing to do
                    }
                }

                throw;
            }
            finally
            {
                if (File.Exists(backupFilePath))
                {
                    try
                    {
                        File.Delete(backupFilePath);
                    }
                    catch
                    {
                        //nothing to do
                    }
                }
            }
        }

        private void Cleanup(
            )
        {
            var border = DateTime.Now.AddDays(-Math.Abs(_storeDays));

            _list.RemoveAll(j => j.SyncDate <= border);
        }

        private List<SyncReport> ReadOrCreate(
            )
        {
            List<SyncReport> result;

            var fi = new FileInfo(_filePath);
            if (fi.Exists)
            {
                using (var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
                {
                    result = _serializer.Deserialize<List<SyncReport>>(
                        fs
                        );
                }
            }
            else
            {
                result = new List<SyncReport>();
            }

            if (!Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }

            return
                result;
        }
    }
}
