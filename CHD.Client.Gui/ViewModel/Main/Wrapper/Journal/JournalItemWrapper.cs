using System;
using CHD.Common.Sync.Report;
using CHD.WcfChannel.Journal;

namespace CHD.Client.Gui.ViewModel.Main.Wrapper.Journal
{
    public sealed class JournalItemWrapper
    {
        public WcfHistorySyncReport Report
        {
            get;
            private set;
        }

        public string Header
        {
            get
            {
                return
                    string.Format(
                        "{0}:    {1}", 
                        SyncDate,
                        SyncResult
                        );
            }
        }

        public string SyncDate
        {
            get
            {
                return
                    Report.SyncDate.ToString("yyyy.MM.dd HH:mm:ss");
            }
        }

        public string SyncResult
        {
            get
            {
                return
                    Report.SyncResult.ToHumanReadableString();
            }
        }

        public string LocalStatString
        {
            get
            {
                return
                    Report.Local.StatString;
            }
        }

        public string RemoteStatString
        {
            get
            {
                return
                    Report.Remote.StatString;
            }
        }

        public JournalItemWrapper(
            WcfHistorySyncReport report
            )
        {
            if (report == null)
            {
                throw new ArgumentNullException("report");
            }

            Report = report;
        }
    }
}