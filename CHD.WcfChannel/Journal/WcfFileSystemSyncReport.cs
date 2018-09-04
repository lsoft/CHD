using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using CHD.Common.Sync.Report;

namespace CHD.WcfChannel.Journal
{
    [DataContract]
    public sealed class WcfFileSystemSyncReport
    {
        public int TotalTouched
        {
            get
            {
                return
                    FilesTouchedCount + FoldersTouchedCount;
            }
        }

        public int FilesTouchedCount
        {
            get
            {
                return
                    DeletedFilesCount + CreatedFilesCount + RecreatedFilesCount;
            }
        }

        [DataMember]
        public int DeletedFilesCount
        {
            get;
            private set;
        }

        [DataMember]
        public int CreatedFilesCount
        {
            get;
            private set;
        }

        [DataMember]
        public int RecreatedFilesCount
        {
            get;
            private set;
        }

        public int FoldersTouchedCount
        {
            get
            {
                return
                    DeletedFoldersCount + CreatedFoldersCount + RecreatedFoldersCount;
            }
        }

        [DataMember]
        public int DeletedFoldersCount
        {
            get;
            private set;
        }

        [DataMember]
        public int CreatedFoldersCount
        {
            get;
            private set;
        }

        [DataMember]
        public int RecreatedFoldersCount
        {
            get;
            private set;
        }

        [DataMember]
        public List<WcfSingleOperationReport> Operations
        {
            get;
            private set;
        }

        public string StatString
        {
            get
            {
                return
                    string.Format(
                        "{0}/{1}/{2} {3}/{4}/{5}", 
                        CreatedFilesCount,
                        RecreatedFilesCount,
                        DeletedFilesCount,
                        CreatedFoldersCount,
                        RecreatedFoldersCount,
                        DeletedFoldersCount
                        );
            }
        }

        //public WcfFileSystemSyncReport()
        //{
        //    DeletedFilesCount = 0;
        //    CreatedFilesCount = 0;
        //    RecreatedFilesCount = 0;

        //    DeletedFoldersCount = 0;
        //    CreatedFoldersCount = 0;
        //    RecreatedFoldersCount = 0;
        //}

        //public WcfFileSystemSyncReport(
        //    int deletedFilesCount, 
        //    int createdFilesCount, 
        //    int recreatedFilesCount, 
        //    int deletedFoldersCount, 
        //    int createdFoldersCount, 
        //    int recreatedFoldersCount, 
        //    List<WcfSingleOperationReport> operations
        //    )
        //{
        //    if (operations == null)
        //    {
        //        throw new ArgumentNullException("operations");
        //    }

        //    DeletedFilesCount = deletedFilesCount;
        //    CreatedFilesCount = createdFilesCount;
        //    RecreatedFilesCount = recreatedFilesCount;
        //    DeletedFoldersCount = deletedFoldersCount;
        //    CreatedFoldersCount = createdFoldersCount;
        //    RecreatedFoldersCount = recreatedFoldersCount;
        //    Operations = operations;
        //}

        public WcfFileSystemSyncReport(IFileSystemSyncReport report)
        {
            DeletedFilesCount = report.DeletedFilesCount;
            CreatedFilesCount = report.CreatedFilesCount;
            RecreatedFilesCount = report.RecreatedFilesCount;
            DeletedFoldersCount = report.DeletedFoldersCount;
            CreatedFoldersCount = report.CreatedFoldersCount;
            RecreatedFoldersCount = report.RecreatedFoldersCount;
            Operations = report.Operations.Select(j => new WcfSingleOperationReport(j.Type, j.FullPath)).ToList();
        }

        public bool ChangesExists(WcfFileSystemSyncReport report)
        {
            if (report == null)
            {
                throw new ArgumentNullException("report");
            }

            if(this.DeletedFilesCount != report.DeletedFilesCount)
            {
                return true;
            }

            if(this.CreatedFilesCount != report.CreatedFilesCount)
            {
                return true;
            }

            if(this.RecreatedFilesCount != report.RecreatedFilesCount)
            {
                return true;
            }

            if(this.DeletedFoldersCount != report.DeletedFoldersCount)
            {
                return true;
            }

            if(this.CreatedFoldersCount != report.CreatedFoldersCount)
            {
                return true;
            }

            if(this.RecreatedFoldersCount != report.RecreatedFoldersCount)
            {
                return true;
            }

            if (this.Operations.Count != report.Operations.Count)
            {
                return true;
            }

            var zip = this.Operations.Zip(report.Operations, (a, b) => new Tuple<WcfSingleOperationReport, WcfSingleOperationReport>(a, b));

            if (zip.Any(j => j.Item1.ChangesExists(j.Item2)))
            {
                return true;
            }

            return false;
        }
    }
}