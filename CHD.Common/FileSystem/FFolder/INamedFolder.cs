using System;
using CHD.Common.Others;

namespace CHD.Common.FileSystem.FFolder
{
    public interface INamedFolder
    {
        string Name
        {
            get;
        }

        string FullPath
        {
            get;
        }

        PathSequence FullPathSequence
        {
            get;
        }

        Guid ChangeIdentifier
        {
            get;
        }
    }
}