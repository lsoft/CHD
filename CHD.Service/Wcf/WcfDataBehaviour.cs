using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using CHD.Common;
using CHD.Common.OnlineStatus.Diff.Apply;
using CHD.Common.OnlineStatus.Diff.Build;
using CHD.Common.OnlineStatus.Sync;
using CHD.Common.Sync.Report.Journal;

namespace CHD.Service.Wcf
{
    public sealed class WcfDataBehaviour : IServiceBehavior
    {
        private readonly ServiceSettings _serviceSettings;
        private readonly ISyncOnlineReport _syncOnlineReport;
        private readonly IDiffBuildOnlineReport _diffBuildOnlineReport;
        private readonly IDiffApplyOnlineReport _diffApplyOnlineReport;
        private readonly ISyncJournal _syncJournal;

        public WcfDataBehaviour(
            ServiceSettings serviceSettings,
            ISyncOnlineReport syncOnlineReport,
            IDiffBuildOnlineReport diffBuildOnlineReport,
            IDiffApplyOnlineReport diffApplyOnlineReport,
            ISyncJournal syncJournal
            )
        {
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

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var cdb in serviceHostBase.ChannelDispatchers)
            {
                var cd = cdb as ChannelDispatcher;
                if (cd != null)
                {
                    foreach (var ed in cd.Endpoints)
                    {
                        ed.DispatchRuntime.InstanceProvider = new WcfDataFactory(
                            _serviceSettings,
                            _syncOnlineReport,
                            _diffBuildOnlineReport,
                            _diffApplyOnlineReport,
                            _syncJournal
                            );
                    }
                }
            }
        }

        public void Validate(
            ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase)
        {
        }

        public void AddBindingParameters(
            ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase,
            Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters
            )
        {
        }
    }
}