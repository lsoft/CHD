using System;
using System.IO;
using CHD.FileSystem.Algebra;

namespace CHD.Graveyard.RemoteFileState
{
    public interface IRemoteFileState
    {
        long Order
        {
            get;
        }

        Suffix FilePathSuffix
        {
            get;
        }

        bool ShouldBeDeleted
        {
            get;
        }

        void WriteTo(
            Stream destination,
            Action<int, int> progressChangeFunc = null
            );
    }
}