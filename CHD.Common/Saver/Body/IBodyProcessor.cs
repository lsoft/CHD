using System.IO;
using CHD.Common.FileSystem;
using CHD.Common.FileSystem.FFile;
using CHD.Common.Structure;

namespace CHD.Common.Saver.Body
{
    public interface IBodyProcessor
    {
        /// <summary>
        /// Удалить те файлы (те леттеры с файлами):
        /// 1) у которых неполноценные снепшоты (например нет  MessageTypeEnum.CloseFile)
        /// 2) которых уже нет ни в одной из сохраненных версий структуры
        /// 3) которые сохранены для "будущей" несуществующей версии структуры
        /// </summary>
        void Cleanup(
            StructureChecker checker
            );

        void SaveNewSnapshot(
            int structureCurrentVersion,
            IFileSystemCopier copier,
            ICopyableFile sourceFile, //remote filesystem of unknown type
            IFile targetFile
            );

        void DeleteLastSnapshot(
            INamedFile file
            );

        long CopySnapshotTo(
            int structureVersion,
            ICopyableFile sourceFile,
            Stream destinationStream,
            long position,
            long size
            );

        long CopyLastSnapshotTo(
            ICopyableFile sourceFile,
            Stream destinationStream,
            long position,
            long size
            );

    }
}