using CHD.Common.FileSystem.FFile;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.Operation.Fixer;

namespace CHD.Common.FileSystem
{
    public interface IFileSystemExecutor
    {
        IFixer<IFolder> CreateChildFolder(
            INamedFolder sourceFileSystemFolder,
            IFolder targetFileSystemParentFolder
            );

        IFixer<IFolder> DeleteChildFolder(
            INamedFolder sourceFileSystemFolder,
            IFolder targetFileSystemParentFolder
            );

        IFixer<IFile> CreateOrUpdateFile(
            IFileSystemCopier sourceFileSystemCopier,
            ICopyableFile sourceFileSystemFile,
            IFolder targetFileSystemParentFolder
            );

        IFixer<IFile> DeleteFile(
            INamedFile sourceFileSystemFile,
            IFolder targetFileSystemParentFolder
            );
    }
}