using System;
using System.Collections.Generic;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.Operation;
using CHD.Common.PathComparer;
using CHD.Common.Serializer;

namespace CHD.Common.Structure
{
    public interface IStoredStructure
    {
        int Version
        {
            get;
        }

        bool IsReadOnly
        {
            get;
        }

        IFolder RootFolder
        {
            get;
        }

        Guid GetChangeIdentifier(
            );

        void SetReadOnlyMode(
            );

        IStoredStructure CreateNew(
            IFolder actualRoot
            );
    }
}