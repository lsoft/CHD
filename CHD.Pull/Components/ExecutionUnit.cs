using System;
using System.IO;
using System.Threading;
using CHD.Common.Logger;
using CHD.FileSystem.Algebra;
using CHD.FileSystem.Watcher;
using CHD.Graveyard.RemoteFileState;

namespace CHD.Pull.Components
{
    public class ExecutionUnit : IExecutionUnit
    {
        private readonly IDisorderLogger _logger;

        private int _disposed = 0;

        private readonly IRemoteFileState _fileState;
        private readonly ExclusiveFileWorker _fileWorker;
        private readonly ExcluderWrapper _fileExcluder;

        public Suffix FilePathSuffix
        {
            get
            {
                return
                    _fileState.FilePathSuffix;
            }
        }

        public long Order
        {
            get
            {
                return
                    _fileState.Order;
            }
        }

        public ExecutionUnit(
            string targetFolder,
            IRemoteFileState fileState,
            IExcluder excluder,
            IDisorderLogger logger
            )
        {
            if (targetFolder == null)
            {
                throw new ArgumentNullException("targetFolder");
            }
            if (fileState == null)
            {
                throw new ArgumentNullException("fileState");
            }
            if (excluder == null)
            {
                throw new ArgumentNullException("excluder");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _logger = logger;

            var filepath = Path.Combine(
                targetFolder,
                fileState.FilePathSuffix.FilePathSuffix
                );

            var fileWriter = new ExclusiveFileWorker(
                filepath,
                logger
                );

            fileWriter.LockFileIsExists();

            try
            {
                var fileExcluder = new ExcluderWrapper(
                    excluder,
                    filepath
                    );

                fileWriter.LockFileAnyway();

                _fileState = fileState;
                _fileWorker = fileWriter;
                _fileExcluder = fileExcluder;
            }
            catch
            {
                fileWriter.Dispose();
                throw;
            }

        }

        public void PerformOperation(
            Action<int, int> progressChangeFunc = null
            )
        {
            if (_fileState.ShouldBeDeleted)
            {

                DeleteFile(progressChangeFunc);
            }
            else
            {
                UpdateFileBody(progressChangeFunc);
            }
        }

        public void Dispose()
        {
            var disposed = Interlocked.Exchange(ref _disposed, 1);
            if (disposed == 0)
            {
                try
                {
                    _fileWorker.Dispose();
                }
                catch (Exception excp)
                {
                    _logger.LogException(excp);
                }

                try
                {
                    _fileExcluder.Dispose();
                }
                catch (Exception excp)
                {
                    _logger.LogException(excp);
                }
            }
        }

        #region private code

        private void UpdateFileBody(
            Action<int, int> progressChangeFunc = null
            )
        {
            progressChangeFunc = progressChangeFunc ?? ((a, b) => { });

            _fileWorker.UpdateBody(
                _fileState,
                progressChangeFunc
                );
        }

        private void DeleteFile(
            Action<int, int> progressChangeFunc = null
            )
        {
            progressChangeFunc = progressChangeFunc ?? ((a, b) => { });

            progressChangeFunc(0, 1);

            _fileWorker.Delete();

            progressChangeFunc(1, 1);
        }

        #endregion
    }
}