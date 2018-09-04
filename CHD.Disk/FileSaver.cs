using System;
using System.IO;
using CHD.Common.Saver;
using CHD.Common.Serializer;

namespace CHD.Disk
{
    public sealed class FileSaver : IBinarySaver<NotVersionedFileAddress>
    {
        private readonly ISerializer _serializer;

        public ISerializer Serializer
        {
            get
            {
                return _serializer;
            }
        }

        public FileSaver(
            ISerializer serializer
            )
        {
            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            _serializer = serializer;
        }

        public bool IsTargetExists(
            NotVersionedFileAddress address
            )
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            return
                File.Exists(address.FilePath);
        }

        public void Save<T>(
            NotVersionedFileAddress address,
            T savedObject
            )
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            using (var fs = new FileStream(address.FilePath, FileMode.Create, FileAccess.Write))
            {
                _serializer.Serialize(
                    savedObject,
                    fs
                    );
            }
        }

        public T Read<T>(
            NotVersionedFileAddress address
            )
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            using (var fs = new FileStream(address.FilePath, FileMode.Open, FileAccess.Read))
            {
                T result = _serializer.Deserialize<T>(fs);

                return result;
            }
        }
    }
}