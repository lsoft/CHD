using System;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.Others;

namespace CHD.Common.FileSystem.FFile
{
    public interface IFile : ICopyableFile
    {        
        PathSequence FullPathSequence
        {
            get;
        }

        IFolder Folder
        {
            get;
        }

        bool IsNameEquals(
            IFile file
            );

        bool IsNameEquals(
            string fileName
            );

        bool IsSame(
            IFile file
            );

        bool IsSame(
            string fullPath
            );
    }
}