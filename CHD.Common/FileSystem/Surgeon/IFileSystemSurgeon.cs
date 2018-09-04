using System;
using CHD.Common.FileSystem.FFile;
using CHD.Common.FileSystem.FFolder;

namespace CHD.Common.FileSystem.Surgeon
{
    public interface IFileSystemSurgeon : IDisposable
    {
        IFileSystem FileSystem
        {
            get;
        }

        IFolder CreateFolder(
            IFolder folder, 
            string folderName
            );

        IFile CreateFileWithBody(
            IFolder folder,
            string fileName,
            byte[] fileBody,
            DateTime lastWriteTimeUtc = default(DateTime)
            );

        IFile ReplaceFileBody(
            IFolder folder, 
            string fileName, 
            Func<long, byte[]> bodyProvider,
            DateTime lastWriteTimeUtc = default(DateTime)
            );

        IFile DeleteFile(
            IFolder folder, 
            string fileName
            );

        IFolder DeleteFolder(
            IFolder folder,
            string folderName
            );

        void SafelyCommit(
            );
    }
}
