using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using CHD.WcfChannel.Common;
using CHD.WcfChannel.Journal;
using CHD.WcfChannel.Sync;

namespace CHD.WcfChannel
{
    [ServiceContract]
    public interface IWcfDataChannel
    {
        [OperationContract]
        WcfCommonInfo GetCommonInfo(
            );

        [OperationContract]
        WcfSyncReport GetSyncInfo(
            );

        [OperationContract]
        List<WcfHistorySyncReport> GetJournalInfo(
            DateTime? since
            );
    }
}
