using System;
using CHD.Common;
using CHD.WcfChannel.Sync;

namespace CHD.Client.Gui.ViewModel.Main.Wrapper.Sync
{
    public sealed class DiffBuildWrapper
    {
        public WcfDiffBuildReport Report
        {
            get;
            private set;
        }

        public string Status
        {
            get
            {
                return
                    UnspecifiedHelper.ToHumanReadableString(Report.IsInProgress, Report.IsCompleted);
            }
        }

        public DiffBuildWrapper(WcfDiffBuildReport report)
        {
            if (report == null)
            {
                throw new ArgumentNullException("report");
            }
            Report = report;
        }
    }
}