using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHD.Common.Others;

namespace CHD.Common.KeyValueContainer
{
    public sealed class FileKeyValueContainer : IKeyValueContainer
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
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (key.Contains(false, new[] { '\r', '\n' }))
            {
                throw new ArgumentException(
                    string.Format(
                        "Key {0} contains \\r or \\n",
                        key
                        )
                    );
            }

            lock (_locker)
            {
                return
                    _container.TryGetValue(key, out value);
            }
        }

        public void AddOrUpdate(string key, string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (key.Contains(false, new[] {'\r', '\n'}))
            {
                throw new ArgumentException(
                    string.Format(
                        "Key {0} contains \\r or \\n",
                        key
                        )
                    );
            }

            lock (_locker)
            {
                _container[key] = value;

                var fi = Base64Helper.EncodeToString(key);
                var filePath = Path.Combine(_folderPath, fi);
                File.WriteAllLines(filePath,
                    new []
                    {
                        key,
                        value
                    });
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
                    var lines = File.ReadAllLines(file);
                    var value = lines[1];

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
