using System;
using System.IO;
using System.Threading;
using CHD.Common.Logger;
using CHD.Graveyard.ExclusiveAccess;
using CHD.Graveyard.Graveyard;
using CHD.Graveyard.RemoteFile;
using CHD.Push.ActivityPool;
using CHD.Push.Pusher;

namespace CHD.Push.Task
{
    internal class CreateOrChangeAlgorithm : BaseAlgorithm
    {
        private readonly IGraveyard _graveyard;
        private readonly long _maxFileBlockSize;
        private readonly long _minFileBlockSize;

        private FileStream _stream;
        private long _size;

        private IExclusiveAccess _exclusiveAccess;
        private IRemoteFile _remoteFile;

        private long _currentPosition;

        public CreateOrChangeAlgorithm(
            IPusher pusher,
            IGraveyard graveyard,
            Guid taskGuid,
            long maxFileBlockSize,
            long minFileBlockSize,
            int pushTimeoutAfterFailureMsec,
            IDisorderLogger logger
            )
            : base(pusher, taskGuid, pushTimeoutAfterFailureMsec, logger)
        {
            if (graveyard == null)
            {
                throw new ArgumentNullException("graveyard");
            }

            _graveyard = graveyard;
            _maxFileBlockSize = maxFileBlockSize;
            _minFileBlockSize = minFileBlockSize;
        }

        protected override void DoLocalPreparation()
        {
            _stream = new FileStream(_pusher.FileWrapper.FilePath, FileMode.Open, FileAccess.Read, FileShare.Delete);
            _size = _stream.Length;
        }

        protected override void DoRemotePreparation(
            out RemotePreparationResultEnum result
            )
        {
            if (!_graveyard.TryGetExclusiveAccess(out _exclusiveAccess))
            {
                result = RemotePreparationResultEnum.ShouldRepeat;
                return;
            }

            if (_exclusiveAccess.ContainsTransaction(this.TaskGuid))
            {
                result = RemotePreparationResultEnum.AlreadyCommitted;
                return;
            }
            
            _remoteFile = _exclusiveAccess.OpenRemoteFile(
                this.TaskGuid,
                _pusher.FileWrapper,
                _size
                );

            result = RemotePreparationResultEnum.AllowToContinue;
        }

        protected override float DoRemoteIteration(
            )
        {
            var position = _currentPosition;
            var tailSize = _size - position;

            var onePercentSize = _size / 100;
            var blockSize = Math.Max(_minFileBlockSize, onePercentSize); //не меньше минимума
            blockSize = Math.Min(_maxFileBlockSize, blockSize); //не больше максимума
            blockSize = Math.Min(tailSize, blockSize); //не больше остатка файла

            if (blockSize <= 0)
            {
                return
                    1f;
            }
            
            _stream.Position = position;

            var buffer = new byte[blockSize];
            _stream.Read(buffer, 0, (int) blockSize);

            _remoteFile.StoreBlock(
                buffer
                );

            _currentPosition += blockSize;

            if (_currentPosition == _size)
            {
                return 1f;
            }

            return
                (float) _currentPosition/_size;
        }

        protected override void SafelyCloseRemoteInfrastructure()
        {
            SafelyCloseRemoteFile();
            SafelyCloseExclusiveAccess();
        }

        protected override void SafelyCloseLocalInfrastructure()
        {
            SafelyCloseStream();
        }

        private void SafelyCloseStream()
        {
            var stream = Interlocked.Exchange(ref _stream, null);
            if (stream != null)
            {
                try
                {
                    stream.Close();
                }
                catch (Exception excp)
                {
                    _logger.LogException(excp);
                }
            }
        }

        private void SafelyCloseRemoteFile()
        {
            var remoteFile = Interlocked.Exchange(ref _remoteFile, null);
            if (remoteFile != null)
            {
                try
                {
                    remoteFile.Close();
                }
                catch (Exception excp)
                {
                    _logger.LogException(excp);
                }
            }
        }

        private void SafelyCloseExclusiveAccess()
        {
            var exclusiveAccess = Interlocked.Exchange(ref _exclusiveAccess, null);
            if (exclusiveAccess != null)
            {
                try
                {
                    exclusiveAccess.Close();
                }
                catch (Exception excp)
                {
                    _logger.LogException(excp);
                }
            }
        }
    }
}