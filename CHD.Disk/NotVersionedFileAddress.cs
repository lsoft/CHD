using System;
using System.Collections.Generic;
using CHD.Common.Native;
using CHD.Common.Saver;

namespace CHD.Disk
{
    public sealed class NotVersionedFileAddress : IAddress
    {
        public string FilePath
        {
            get;
            private set;
        }

        public string TargetFolder
        {
            get
            {
                throw new InvalidOperationException("Not applicable");
            }
        }

        public Func<string, bool> Filter
        {
            get
            {
                throw new InvalidOperationException("Not applicable");
            }
        }

        public NotVersionedFileAddress(
            string filePath
            )
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            FilePath = filePath;
        }

        public IComparer<TNativeMessage> GetComparer<TNativeMessage>()
            where TNativeMessage : NativeMessage
        {
            throw new InvalidOperationException("Not applicable");
        }

        public string ComposeNewSubject(string oldSubject)
        {
            throw new InvalidOperationException("Not applicable");
        }

        public string ComposeNewSubject()
        {
            throw new InvalidOperationException("Not applicable");
        }
    }
}