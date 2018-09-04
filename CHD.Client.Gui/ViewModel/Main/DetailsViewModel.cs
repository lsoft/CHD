using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using CHD.Client.Gui.DataFlow;
using CHD.Client.Gui.ViewModel.Main.Wrapper.Journal;
using CHD.Client.Gui.ViewModel.Main.Wrapper.Sync;
using CHD.WcfChannel;
using CHD.WcfChannel.Journal;
using CHD.Wpf;

namespace CHD.Client.Gui.ViewModel.Main
{
    public sealed class DetailsViewModel : BaseViewModel
    {
        public JournalItemWrapper Wrapper
        {
            get;
            private set;
        }

        public DetailsViewModel(
            Dispatcher dispatcher,
            JournalItemWrapper wrapper
            )
            : base(dispatcher)
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }

            Wrapper = wrapper;
        }
    }
}
