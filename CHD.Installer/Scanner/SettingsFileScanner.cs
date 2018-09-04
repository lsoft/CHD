using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using CHD.Common;
using CHD.Common.Others;
using CHD.Installer.Scanner.Euristic;

namespace CHD.Installer.Scanner
{
    internal sealed class SettingsFileScanner
    {
        private readonly ISettingsEuristic _euristic;
        private readonly string _folderPath;

        public SettingsFileScanner(
            ISettingsEuristic euristic,
            string folderPath
            )
        {
            if (euristic == null)
            {
                throw new ArgumentNullException("euristic");
            }
            if (folderPath == null)
            {
                throw new ArgumentNullException("folderPath");
            }
            _euristic = euristic;
            _folderPath = folderPath;
        }

        public bool TryToFindSettingsFile(
            out string filePath
            )
        {
            var result = false;
            filePath = null;

            if (Directory.Exists(_folderPath))
            {
                var fileInfos = Directory
                    .GetFiles(_folderPath, "*.xml", SearchOption.AllDirectories).Select(j => new FileInfo(j))
                    .Where(j => j.Name.NotContains(true, "Ninject", "Castle.Core.xml"))
                    .ToList();

                filePath = _euristic.Filter(fileInfos);

                result = true;
            }

            return
                result;
        }
    }
}
