using System;
using System.IO;
using System.Threading;
using CHD.Common.Logger;
using CHD.Graveyard.RemoteFileState;

namespace CHD.Pull.Components
{
    public class ExclusiveFileWorker : IDisposable
    {
        private readonly string _filepath;
        private readonly IDisorderLogger _logger;

        private FileStream _stream;

        private volatile int _locked = 0;

        private int _disposed = 0;


        public ExclusiveFileWorker(
            string filepath,
            IDisorderLogger logger
            )
        {
            if (filepath == null)
            {
                throw new ArgumentNullException("filepath");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _filepath = filepath;
            _logger = logger;
        }

        public void LockFileIsExists()
        {
            if (_locked == 0)
            {
                var result = Lock(FileMode.Open);
                if (result)
                {
                    _locked = 1;
                }
            }
        }

        public void LockFileAnyway()
        {
            if (_locked == 0)
            {
                var result = Lock(FileMode.OpenOrCreate);
                if (result)
                {
                    _locked = 1;
                }
            }
        }

        public void UpdateBody(
            IRemoteFileState fileState,
            Action<int, int> progressChangeFunc = null
            )
        {
            if (fileState == null)
            {
                throw new ArgumentNullException("fileState");
            }

            progressChangeFunc = progressChangeFunc ?? ((a, b) => { });

            var tempFilePath = Path.Combine(
                Path.GetTempPath(),
                Path.GetRandomFileName()
                );

            int ttl = 0;
            using (var tempFile = new FileStream(tempFilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Delete))
            {
                try
                {
                    //write network data into temp stream
                    //it's a time-consuming operation and it can suffer from network failures
                    //in case of any kind of failure nothing bad happens with our target (local) file
                    fileState.WriteTo(
                        tempFile,
                        (cnt, total) =>
                        {
                            ttl = total + 1;

                            progressChangeFunc(cnt, ttl);
                        }
                        );

                    tempFile.Position = 0;

                    //it'a a time-consuming operation too
                    //but it's a local operation so it's faster than previous network-copy operation
                    //so there is a some degree of risk anyway
                    tempFile.CopyTo(_stream);

                    progressChangeFunc(ttl, ttl);
                }
                finally
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        public void Delete()
        {
            System.IO.File.Delete(_filepath);

            _logger.LogFormattedMessage(
                "Local file deleted: {0}",
                _filepath
                );
        }

        public void Dispose()
        {
            var disposed = Interlocked.Exchange(ref _disposed, 1);
            if (disposed == 0)
            {
                var stream = Interlocked.Exchange(ref _stream, null);

                //stream.Unlock(0, stream.Length);

                stream.Flush();
                stream.Close();
                stream.Dispose();
            }
        }

        private bool Lock(
            FileMode fileMode
            )
        {
            var result = false;

            try
            {
                _stream = new FileStream(_filepath, fileMode, FileAccess.Write, FileShare.Delete);
                //_stream.Lock(0, _stream.Length);

                result = true;
            }
            catch (FileNotFoundException)
            {
                //nothing should be performed here
            }

            return
                result;
        }
    }
}