using System;
using System.Linq;
using CHD.Common;
using CHD.Common.OnlineStatus.Diff.Apply.Report;
using CHD.WcfChannel.Sync;
using CHD.Wpf;

namespace CHD.Client.Gui.ViewModel.Main.Wrapper.Sync
{
    public sealed class DiffApplyWrapper
    {
        public WcfDiffApplyReport Report
        {
            get;
            set;
        }

        public string Status
        {
            get
            {
                return
                    UnspecifiedHelper.ToHumanReadableString(Report.IsInProgress, Report.IsCompleted);
            }
        }

        public ObservableCollection2<OperationWrapper> Operations
        {
            get;
            private set;
        }

        public string ProgressCaption
        {
            get
            {
                return
                    UnspecifiedHelper.ToHumanReadableString(Report.IsInProgress, Report.IsCompleted);
            }
        }

        public double Progress
        {
            get
            {
                if (Operations.Count == 0)
                {
                    return 0;
                }

                var completed = Operations.Count(j => j.Report.Status == OperationReportStatusEnum.SuccessfullyCompleted);

                var result = completed / (double)Operations.Count;

                return
                    result * 100;
            }
        }

        public DiffApplyWrapper(WcfDiffApplyReport report)
        {
            if (report == null)
            {
                throw new ArgumentNullException("report");
            }

            Report = report;

            Operations = new ObservableCollection2<OperationWrapper>();
            Operations.AddRange(report.Operations.Select(j => new OperationWrapper(j)));
        }
    }
}