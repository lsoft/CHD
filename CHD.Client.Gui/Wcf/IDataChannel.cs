using System;
using System.Collections.Generic;
using CHD.Client.Gui.ViewModel.Main;
using CHD.Client.Gui.ViewModel.Main.Wrapper.Common;
using CHD.Client.Gui.ViewModel.Main.Wrapper.Journal;
using CHD.Client.Gui.ViewModel.Main.Wrapper.Sync;
using CHD.WcfChannel.Journal;

namespace CHD.Client.Gui.Wcf
{
    public interface IDataChannel : IDisposable
    {
        CommonWrapper GetCommonInfo(
            );

        SyncWrapper GetSyncInfo(
            );

        List<WcfHistorySyncReport> GetJournalInfo(
            DateTime? since
            );
    }
}