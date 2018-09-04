using System.Collections.Generic;

namespace CHD.Common.Sync.Report.Journal
{
    public interface ISyncJournal
    {
        IReadOnlyList<SyncReport> Reports
        {
            get;
        }

        void Add(
            SyncReport report
            );
    }
}