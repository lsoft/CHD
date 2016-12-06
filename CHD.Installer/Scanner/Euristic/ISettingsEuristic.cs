using System.Collections.Generic;
using System.IO;

namespace CHD.Installer.Scanner.Euristic
{
    internal interface ISettingsEuristic
    {
        string Filter(List<FileInfo> fileInfos);
    }
}