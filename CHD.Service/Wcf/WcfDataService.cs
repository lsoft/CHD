using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using CHD.Common.OnlineStatus.Diff.Apply;
using CHD.Common.OnlineStatus.Diff.Build;
using CHD.Common.OnlineStatus.Sync;
using CHD.Common.Sync.Report.Journal;
using CHD.WcfChannel;
using CHD.WcfChannel.Common;
using CHD.WcfChannel.Journal;
using CHD.WcfChannel.Sync;

namespace CHD.Service.Wcf
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
    public sealed class WcfDataService : IWcfDataChannel
    {
        private readonly ServiceSettings _serviceSettings;
        private readonly ISyncOnlineReport _syncOnlineReport;
        private readonly IDiffBuildOnlineReport _diffBuildOnlineReport;
        private readonly IDiffApplyOnlineReport _diffApplyOnlineReport;
        private readonly ISyncJournal _syncJournal;

        public WcfDataService(
            ServiceSettings serviceSettings,
            ISyncOnlineReport syncOnlineReport,
            IDiffBuildOnlineReport diffBuildOnlineReport,
            IDiffApplyOnlineReport diffApplyOnlineReport,
            ISyncJournal syncJournal
            )
        {
            if (serviceSettings == null)
            {
                throw new ArgumentNullException("serviceSettings");
            }
            if (syncOnlineReport == null)
            {
                throw new ArgumentNullException("syncOnlineReport");
            }
            if (diffBuildOnlineReport == null)
            {
                throw new ArgumentNullException("diffBuildOnlineReport");
            }
            if (diffApplyOnlineReport == null)
            {
                throw new ArgumentNullException("diffApplyOnlineReport");
            }
            if (syncJournal == null)
            {
                throw new ArgumentNullException("syncJournal");
            }
            _serviceSettings = serviceSettings;
            _syncOnlineReport = syncOnlineReport;
            _diffBuildOnlineReport = diffBuildOnlineReport;
            _diffApplyOnlineReport = diffApplyOnlineReport;
            _syncJournal = syncJournal;
        }

        public WcfCommonInfo GetCommonInfo()
        {
            var result = new WcfCommonInfo(
                Path.GetFullPath(_serviceSettings.WatchFolder)
                );

            return
                result;
        }

        public WcfSyncReport GetSyncInfo()
        {
            var result = new WcfSyncReport(
                _syncOnlineReport.IsInProgress,
                _syncOnlineReport.IsCompleted,
                new WcfDiffBuildReport(
                    _diffBuildOnlineReport.IsInProgress,
                    _diffBuildOnlineReport.IsCompleted
                    ),
                new WcfDiffApplyReport(
                    _diffApplyOnlineReport.IsInProgress,
                    _diffApplyOnlineReport.IsCompleted,
                    _diffApplyOnlineReport.OperationReports
                    )
                );

            return
                result;
        }

        public List<WcfHistorySyncReport> GetJournalInfo(DateTime? since)
        {
            var result = _syncJournal.Reports
                .Where(j => !since.HasValue || j.SyncDate > since)
                .Select(j => new WcfHistorySyncReport(
                    j.SyncDate,
                    j.SyncResult,
                    new WcfFileSystemSyncReport(
                        j.Local
                        ),
                    new WcfFileSystemSyncReport(
                        j.Remote
                        )
                    )
                )
                .ToList();

            return
                result;
        }
    }
}
