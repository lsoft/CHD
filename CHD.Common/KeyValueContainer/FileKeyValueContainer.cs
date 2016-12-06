using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHD.Common.Logger;

namespace CHD.Common.KeyValueContainer
{
    public class FileKeyValueContainer : IKeyValueContainer
    {
        private readonly object _locker = new object();

        private readonly string _folderPath;
        private readonly IDisorderLogger _logger;

        private readonly Dictionary<string, string> _container = new Dictionary<string, string>();

        public FileKeyValueContainer(
            string folderPath,
            IDisorderLogger logger
            )
        {
            if (folderPath == null)
            {
                throw new ArgumentNullException("folderPath");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _folderPath = folderPath;
            _logger = logger;

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            ReadData();
        }

        public bool TryGet(string key, out string value)
        {
            lock (_locker)
            {
                return
                    _container.TryGetValue(key, out value);
            }
        }

        public void Add(string key, string value)
        {
            lock (_locker)
            {
                _container[key] = value;

                var fi = Base64Helper.EncodeToString(key);
                var filePath = Path.Combine(_folderPath, fi);
                File.WriteAllText(filePath, value);
            }
        }

        private void ReadData()
        {
            foreach (var file in Directory.GetFiles(_folderPath))
            {
                try
                {
                    var fi = new FileInfo(file);
                    var fin = fi.Name;

                    var key = Base64Helper.DecodeFromString(fin);
                    var value = File.ReadAllText(file);

                    lock (_locker)
                    {
                        _container[key] = value;
                    }
                }
                catch (Exception excp)
                {
                    _logger.LogException(excp);
                }
            }
        }
    }
}
