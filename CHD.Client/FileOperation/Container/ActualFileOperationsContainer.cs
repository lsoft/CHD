using System;
using System.Collections.Concurrent;
using System.Windows.Threading;
using CHD.Client.FileOperation.Pusher;
using CHD.Common.Logger;
using CHD.FileSystem.Algebra;
using CHD.FileSystem.FileWrapper;

namespace CHD.Client.FileOperation.Container
{
    internal class ActualFileOperationsContainer : IActualFileOperationsContainer
    {
        private readonly Dispatcher _dispatcher;
        private readonly IDisorderLogger _logger;

        private readonly ConcurrentDictionary<Suffix, ActualFileOperationWrapper> _dictionary = new ConcurrentDictionary<Suffix, ActualFileOperationWrapper>(new SuffixEqualityComparer()); 

        public event FileOperationChangedDelegate FileOperationChangedEvent;

        public ActualFileOperationsContainer(
            Dispatcher dispatcher,
            IDisorderLogger logger
            )
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _dispatcher = dispatcher;
            _logger = logger;
        }

        public void AddOperation(
            IFileOperation fileOperation,
            FileActionTypeEnum fileActionType
            )
        {
            if (fileOperation == null)
            {
                throw new ArgumentNullException("fileOperation");
            }

            var operation = new ActualFileOperationWrapper(
                _dispatcher,
                fileOperation,
                fileActionType
                );

            var result = _dictionary.TryAdd(fileOperation.FilePathSuffix, operation);

            if (!result)
            {
                throw new InvalidOperationException("TryAdd fails");
            }

            OnFileOperationChanged(true, operation);
        }

        public void StartOperation(
            IFileOperation fileOperation
            )
        {
            if (fileOperation == null)
            {
                throw new ArgumentNullException("fileOperation");
            }

            ActualFileOperationWrapper operation;
            if (_dictionary.TryGetValue(fileOperation.FilePathSuffix, out operation))
            {
                operation.SetIsWorking(true);
            }
        }

        public void RemoveOperation(
            IFileOperation fileOperation
            )
        {
            if (fileOperation == null)
            {
                throw new ArgumentNullException("fileOperation");
            }

            ActualFileOperationWrapper operation;
            if (_dictionary.TryRemove(fileOperation.FilePathSuffix, out operation))
            {
                OnFileOperationChanged(false, operation);

                operation.Dispose();
            }
        }

        protected virtual void OnFileOperationChanged(bool created, ActualFileOperationWrapper operationwrapper)
        {
            try
            {
                var handler = FileOperationChangedEvent;
                if (handler != null)
                {
                    handler(created, operationwrapper);
                }
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);
            }
        }

    }
}