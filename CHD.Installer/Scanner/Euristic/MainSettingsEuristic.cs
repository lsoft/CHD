using System.Collections.Generic;
using System.IO;
using System.Linq;
using CHD.Common;

namespace CHD.Installer.Scanner.Euristic
{
    internal class MainSettingsEuristic : ISettingsEuristic
    {
        public string Filter(List<FileInfo> fileInfos)
        {
            if (fileInfos == null)
            {
                return string.Empty;
            }
            if (fileInfos.Count == 0)
            {
                return string.Empty;
            }
            if (fileInfos.Count == 1)
            {
                return fileInfos[0].FullName;
            }

            var notSuspectedByName = fileInfos.FirstOrDefault(j => j.Name.NotContains(true, "email", "cloud", "local", "MailKit", "MimeKit"));
            if (notSuspectedByName != null)
            {
                return
                    notSuspectedByName.FullName;
            }

            var biggerFile = fileInfos.OrderByDescending(j => j.Length).First();

            return
                biggerFile.FullName;
        }
    }
}