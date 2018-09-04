using System;
using System.IO;

namespace CHD.Common.FileSystem.FFile
{
    public interface ICopyableFile : INamedFile
    {
        long Size
        {
            get;
        }

        DateTime LastWriteTimeUtc
        {
            get;
        }

        Guid ChangeIdentifier
        {
            get;
        }

    }
}