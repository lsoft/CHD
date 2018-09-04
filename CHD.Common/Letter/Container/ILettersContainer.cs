using System;
using System.Collections.Generic;
using CHD.Common.FileSystem.FFile;
using CHD.Common.Native;
using CHD.Common.Saver;

namespace CHD.Common.Letter.Container
{
    public interface ILettersContainer<TNativeMessage>
        where TNativeMessage : NativeMessage
    {

        IReadOnlyList<ILetter<TNativeMessage>> Letters
        {
            get;
        }

        long MaxOrder
        {
            get;
        }

        List<Tuple<string, List<FileSnapshot<TNativeMessage>>>> GetSnapshots(
            );

        List<FileSnapshot<TNativeMessage>> GetSnapshots(
            INamedFile sourceFile
            );
    }
}