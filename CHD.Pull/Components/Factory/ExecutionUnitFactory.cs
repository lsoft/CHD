using System;
using CHD.Common.Logger;
using CHD.FileSystem.Watcher;
using CHD.Graveyard.RemoteFileState;

namespace CHD.Pull.Components.Factory
{
    public class ExecutionUnitFactory : IExecutionUnitFactory
    {
        private readonly string _targetFolder;
        private readonly IExcluder _excluder;
        private readonly IDisorderLogger _logger;

        public ExecutionUnitFactory(
            string targetFolder,
            IExcluder excluder,
            IDisorderLogger logger
            )
        {
            if (targetFolder == null)
            {
                throw new ArgumentNullException("targetFolder");
            }
            if (excluder == null)
            {
                throw new ArgumentNullException("excluder");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _targetFolder = targetFolder;
            _excluder = excluder;
            _logger = logger;
        }

        public IExecutionUnit Create(
            IRemoteFileState fileState
            )
        {
            if (fileState == null)
            {
                throw new ArgumentNullException("fileState");
            }

            var result = new ExecutionUnit(
                _targetFolder,
                fileState,
                _excluder,
                _logger
                );

            return
                result;
        }
    }
}