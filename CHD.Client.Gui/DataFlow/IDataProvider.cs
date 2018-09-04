using CHD.Client.Gui.ViewModel.Main;
using CHD.Client.Gui.ViewModel.Main.Wrapper.Common;
using CHD.Client.Gui.ViewModel.Main.Wrapper.Journal;
using CHD.Client.Gui.ViewModel.Main.Wrapper.Sync;

namespace CHD.Client.Gui.DataFlow
{
    public interface IDataProvider
    {
        CommonWrapper CommonData
        {
            get;
        }

        SyncWrapper SyncData
        {
            get;
        }

        JournalWrapper JournalData
        {
            get;
        }

        event CommonDataChangedDelegate CommonDataChangedEvent;
        event SyncDataChangedDelegate SyncDataChangedEvent;
        event JournalDataChangedDelegate JournalDataChangedEvent;
    }
}