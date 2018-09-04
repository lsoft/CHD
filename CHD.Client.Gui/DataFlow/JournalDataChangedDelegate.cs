using System.Collections.Generic;
using CHD.Client.Gui.ViewModel.Main.Wrapper.Journal;
using CHD.WcfChannel.Journal;

namespace CHD.Client.Gui.DataFlow
{
    public delegate void JournalDataChangedDelegate(List<JournalItemWrapper> newJournalItems);
}