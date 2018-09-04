using System;
using System.Windows;
using CHD.WcfChannel.Sync;

namespace CHD.Client.Gui.ViewModel.Main.Wrapper.Sync
{
    //public sealed class SingleOperationReportWrapper
    //{
    //    public OperationTypeEnum Type
    //    {
    //        get;
    //        private set;
    //    }

    //    public string FullPath
    //    {
    //        get;
    //        private set;
    //    }

    //    public SingleOperationReportWrapper(
    //        WcfSingleOperationReport report
    //        )
    //    {
    //        Type = report.Type;
    //        FullPath = report.FullPath;
    //    }
    //}

    //public sealed class FileSystemSyncReportWrapper
    //{

    //    public int TotalTouched
    //    {
    //        get
    //        {
    //            return
    //                FilesTouchedCount + FoldersTouchedCount;
    //        }
    //    }

    //    public int FilesTouchedCount
    //    {
    //        get
    //        {
    //            return
    //                DeletedFilesCount + CreatedFilesCount + RecreatedFilesCount;
    //        }
    //    }

    //    public int DeletedFilesCount
    //    {
    //        get;
    //        private set;
    //    }

    //    public int CreatedFilesCount
    //    {
    //        get;
    //        private set;
    //    }

    //    public int RecreatedFilesCount
    //    {
    //        get;
    //        private set;
    //    }

    //    public int FoldersTouchedCount
    //    {
    //        get
    //        {
    //            return
    //                DeletedFoldersCount + CreatedFoldersCount + RecreatedFoldersCount;
    //        }
    //    }

    //    public int DeletedFoldersCount
    //    {
    //        get;
    //        private set;
    //    }

    //    public int CreatedFoldersCount
    //    {
    //        get;
    //        private set;
    //    }

    //    public int RecreatedFoldersCount
    //    {
    //        get;
    //        private set;
    //    }

    //    public IReadOnlyList<SingleOperationReportWrapper> Operations
    //    {
    //        get;
    //        private set;
    //    }

    //    public FileSystemSyncReportWrapper(
    //        WcfFileSystemSyncReport report
    //        )
    //    {
    //        if (report == null)
    //        {
    //            throw new ArgumentNullException("report");
    //        }

    //        DeletedFilesCount = report.DeletedFilesCount;
    //        CreatedFilesCount = report.CreatedFilesCount;
    //        RecreatedFilesCount = report.RecreatedFilesCount;
    //        DeletedFoldersCount = report.DeletedFoldersCount;
    //        CreatedFoldersCount = report.CreatedFoldersCount;
    //        RecreatedFoldersCount = report.RecreatedFoldersCount;
    //        Operations = report.Operations.Select(j => new SingleOperationReportWrapper(j)).ToList();
    //    }
    //}

    public sealed class SyncWrapper
    {
        public WcfSyncReport Report
        {
            get;
            private set;
        }

        public DiffBuildWrapper DiffBuild
        {
            get;
            private set;
        }

        public DiffApplyWrapper DiffApply
        {
            get;
            private set;
        }

        public bool IsInProgress
        {
            get
            {
                return
                    Report.IsInProgress;
            }
        }

        public Visibility ProgressBlockVisibility
        {
            get
            {
                return
                    Report.IsInProgress && !Report.IsCompleted ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public SyncWrapper()
        {
            Report = new WcfSyncReport();
            DiffBuild = new DiffBuildWrapper(Report.DiffBuildReport);
            DiffApply = new DiffApplyWrapper(Report.DiffApplyReport);
        }

        public SyncWrapper(
            WcfSyncReport report
            )
        {
            if (report == null)
            {
                throw new ArgumentNullException("report");
            }

            Report = report;

            DiffBuild = new DiffBuildWrapper(report.DiffBuildReport);
            DiffApply = new DiffApplyWrapper(report.DiffApplyReport);
        }

        public bool ChangesExists(SyncWrapper syncData)
        {
            if (syncData == null)
            {
                throw new ArgumentNullException("syncData");
            }

            if (this.Report.ChangesExists(syncData.Report))
            {
                return true;
            }

            return
                false;
        }
    }
}