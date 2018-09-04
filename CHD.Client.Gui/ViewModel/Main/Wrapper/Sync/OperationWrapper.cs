using System;
using CHD.Common.OnlineStatus.Diff.Apply.Report;
using CHD.Common.Operation;
using CHD.WcfChannel.Sync;

namespace CHD.Client.Gui.ViewModel.Main.Wrapper.Sync
{
    public sealed class OperationWrapper
    {
        public WcfOperationReport Report
        {
            get;
            private set;
        }

        public string Type
        {
            get
            {
                return
                    Report.Type.ToHumanReadableString();
            }
        }

        public string Target
        {
            get
            {
                return
                    Report.Target;
            }
        }

        public string Status
        {
            get
            {
                return
                    Report.Status.ToHumanReadableString();
            }
        }

        public OperationWrapper(WcfOperationReport report)
        {
            if (report == null)
            {
                throw new ArgumentNullException("report");
            }

            Report = report;
        }
    }
}