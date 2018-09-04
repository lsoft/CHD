using System.Collections.Generic;
using CHD.Client.Gui.ViewModel.Main;
using CHD.Client.Gui.ViewModel.Main.Wrapper.Common;
using CHD.Client.Gui.ViewModel.Main.Wrapper.Journal;
using CHD.Client.Gui.ViewModel.Main.Wrapper.Sync;
using CHD.WcfChannel.Journal;

namespace CHD.Client.Gui.DataFlow
{
    public interface IDataContainer : IDataProvider
    {
        bool Set(
            CommonWrapper commonData
            );

        bool Set(
            SyncWrapper syncData
            );

        bool Add(
            List<WcfHistorySyncReport> journalItems
            );
    }
}