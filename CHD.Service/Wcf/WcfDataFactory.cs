using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using CHD.Common;
using CHD.Common.OnlineStatus.Diff.Apply;
using CHD.Common.OnlineStatus.Diff.Build;
using CHD.Common.OnlineStatus.Sync;
using CHD.Common.Sync.Report.Journal;

namespace CHD.Service.Wcf
{
    public sealed class WcfDataFactory : IInstanceProvider
    {
        private readonly ServiceSettings _serviceSettings;
        private readonly ISyncOnlineReport _syncOnlineReport;
        private readonly IDiffBuildOnlineReport _diffBuildOnlineReport;
        private readonly IDiffApplyOnlineReport _diffApplyOnlineReport;
        private readonly ISyncJournal _syncJournal;

        public WcfDataFactory(
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

        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return
                new WcfDataService(
                    _serviceSettings,
                    _syncOnlineReport,
                    _diffBuildOnlineReport,
                    _diffApplyOnlineReport,
                    _syncJournal
                    );
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            //nothing to do
        }
    }
}