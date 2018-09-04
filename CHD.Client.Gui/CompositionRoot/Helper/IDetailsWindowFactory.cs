using System.Windows;
using CHD.Client.Gui.ViewModel.Main.Wrapper.Journal;

namespace CHD.Client.Gui.CompositionRoot.Helper
{
    public interface IDetailsWindowFactory
    {
        Window Create(
            JournalItemWrapper wrapper
            );
    }
}