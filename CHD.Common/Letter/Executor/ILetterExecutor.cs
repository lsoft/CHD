using System.Collections.Generic;
using CHD.Common.FileSystem;
using CHD.Common.FileSystem.FFile;
using CHD.Common.Letter.Container;
using CHD.Common.Native;
using CHD.Common.Saver;

namespace CHD.Common.Letter.Executor
{
    public interface ILetterExecutor<TNativeMessage>
        where TNativeMessage : NativeMessage
    {
        ILettersContainer<TNativeMessage> ReadAllLetters(
            );

        void SaveFile(
            int structureCurrentVersion,
            long newMaxOrder,
            IFileSystemCopier copier,
            ICopyableFile sourceFile,
            IFile targetFile
            );

        void DeleteSnapshots(
            IEnumerable<FileSnapshot<TNativeMessage>> snapshot
            );

        void DeleteSnapshot(
            FileSnapshot<TNativeMessage> snapshot
            );

    }
}