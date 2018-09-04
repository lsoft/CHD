using System;
using CHD.Common;
using CHD.Common.FileSystem.Surgeon;
using CHD.Common.PathComparer;

namespace CHD.Tests.FileSystem.Surgeon.Factory
{
    internal sealed class DiskSurgeonFactory : ISurgeonFactory
    {
        private readonly string _rootFolderPath;
        private readonly IPathComparerProvider _pathComparerProvider;
        private readonly IDisorderLogger _logger;

        public DiskSurgeonFactory(
            string rootFolderPath,
            IPathComparerProvider pathComparerProvider,
            IDisorderLogger logger
            )
        {
            if (rootFolderPath == null)
            {
                throw new ArgumentNullException("rootFolderPath");
            }
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _rootFolderPath = rootFolderPath;
            _pathComparerProvider = pathComparerProvider;
            _logger = logger;
        }

        public ISurgeon Surge(
            )
        {
            return
                new DiskSurgeon(
                    _rootFolderPath,
                    _pathComparerProvider,
                    _logger
                    );
        }
    }
}