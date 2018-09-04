using System;
using System.IO;
using CHD.Common.Native;
using CHD.Common.Others;
using CHD.Common.Saver;

namespace CHD.Common.Letter
{
    public interface ILetter<TNativeMessage>
        where TNativeMessage : NativeMessage
    {
        TNativeMessage NativeMessage
        {
            get;
        }

        int StructureVersion
        {
            get;
        }

        long Order
        {
            get;
        }

        Guid TransactionGuid
        {
            get;
        }

        MessageTypeEnum MessageType
        {
            get;
        }

        PathSequence FullPathSequence
        {
            get;
        }

        Guid LetterGuid
        {
            get;
        }

        long Size
        {
            get;
        }
        
        long WriteAttachmentTo(
            Stream destination,
            long position,
            long size
            );

        //void Delete(
        //    );
    }
}