using System;
using System.Collections.Generic;
using System.IO;
using CHD.Common.Native;
using CHD.Common.Saver;

namespace CHD.Common.ServiceCode.Executor
{
    public interface INativeClientEx<TNativeMessage, TSendableMessage>
        where TNativeMessage : NativeMessage
        where TSendableMessage : SendableMessage
    {

        bool TryGetChild(
            string folderName,
            out INativeClientEx<TNativeMessage, TSendableMessage> client
            );

        INativeClientEx<TNativeMessage, TSendableMessage> CreateOrEnterChild(
            string folderName
            );




        List<TNativeMessage> ReadAndFilterMessages(
            Func<string, bool> filter
            );

        void StoreMessage(
            TSendableMessage message
            );

        void DeleteMessages(
            IEnumerable<TNativeMessage> messages
            );

        long DecodeAttachmentTo(
            TNativeMessage message,
            Stream destination,
            long position = 0,
            long size = 0
            );






        bool IsChildFolderExists(
            string folderName
            );

        void CreateChildFolder(
            string folderName,
            out string createdFolderName
            );
        
        bool DeleteChildFolder(
            string folderName,
            out string deletedFolderName
            );



    }
}