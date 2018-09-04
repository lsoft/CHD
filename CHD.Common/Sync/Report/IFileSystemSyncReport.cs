using System.Collections.Generic;

namespace CHD.Common.Sync.Report
{
    public interface IFileSystemSyncReport
    {
        int TotalTouched
        {
            get;
        }

        int FilesTouchedCount
        {
            get;
        }

        int DeletedFilesCount
        {
            get;
        }

        int CreatedFilesCount
        {
            get;
        }

        int RecreatedFilesCount
        {
            get;
        }


        int FoldersTouchedCount
        {
            get;
        }

        int DeletedFoldersCount
        {
            get;
        }

        int CreatedFoldersCount
        {
            get;
        }

        int RecreatedFoldersCount
        {
            get;
        }

        IReadOnlyList<SingleOperationReport> Operations
        {
            get;
        }



        void Dump(IDisorderLogger logger);
    }
}