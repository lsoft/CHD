using System.Collections.Generic;
using System.IO;
using System.Linq;
using CHD.Common;
using CHD.Common.Others;

namespace CHD.Installer.Scanner.Euristic
{
    internal sealed class MailRuCloudSettingsEuristic : ISettingsEuristic
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

            var suspectedByName = fileInfos.FirstOrDefault(j => Helper.Contains(j.Name, true, "cloud"));
            if (suspectedByName != null)
            {
                return
                    suspectedByName.FullName;
            }

            var notBigger = fileInfos.OrderByDescending(j => j.Length).Skip(1).First();

            return
                notBigger.FullName;
        }
    }
}