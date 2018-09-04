using System;
using System.IO;
using CHD.Common;

namespace CHD.Disk.Cleaner
{
    public sealed class DiskFileSystemCleaner : IDiskFileSystemCleaner
    {
        private readonly string _rootFolderPath;
        private readonly string _structureFilePath;
        private readonly IDisorderLogger _logger;

        public DiskFileSystemCleaner(
            string rootFolderPath,
            string structureFilePath,
            IDisorderLogger logger
            )
        {
            if (rootFolderPath == null)
            {
                throw new ArgumentNullException("rootFolderPath");
            }
            if (structureFilePath == null)
            {
                throw new ArgumentNullException("structureFilePath");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _rootFolderPath = rootFolderPath;
            _structureFilePath = structureFilePath;
            _logger = logger;
        }

        public void SafelyClear()
        {
            if (Directory.Exists(_rootFolderPath))
            {
                foreach (var di in Directory.GetDirectories(_rootFolderPath))
                {
                    try
                    {
                        Directory.Delete(di, true);
                    }
                    catch (Exception excp)
                    {
                        _logger.LogException(excp);
                    }
                }

                foreach (var fi in Directory.GetFiles(_rootFolderPath))
                {
                    try
                    {
                        File.Delete(fi);
                    }
                    catch (Exception excp)
                    {
                        _logger.LogException(excp);
                    }
                }
            }

            if (File.Exists(_structureFilePath))
            {
                File.Delete(_structureFilePath);
            }

        }
    }
}
