using CHD.Tests.FileSystem.Schema;

namespace CHD.Tests.FileSystem.Surgeon
{
    public interface ISurgeon
    {
        ISurgeon Enter(
            string folderName
            );

        ISurgeon Enter(
            params string[] folderNames
            );

        ISurgeon Up();
        
        ISurgeon CreateFolder(
            string folderName
            );

        ISurgeon CreatePseudoRandomFile(
            string fileName,
            byte[] body
            );

        ISurgeon ChangeFile(
            string fileName,
            int fileSeed,
            out byte[] newFileBody
            );

        ISurgeon DeleteFile(
            string fileName, 
            bool fileMustExists
            );

        ISurgeon DeleteFolder(
            string folderName,
            bool folderMustExists
            );

        IFileSystemSchema CloseFileSystem();
        
    }
}