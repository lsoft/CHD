using System;
using System.Collections.Generic;
using System.Linq;
using CHD.Common.Others;
using CHD.WcfChannel.Journal;
using CHD.Wpf;

namespace CHD.Client.Gui.ViewModel.Main.Wrapper.Journal
{
    public sealed class JournalWrapper
    {
        public List<JournalItemWrapper> Wrappers
        {
            get;
            private set;
        }

        public DateTime? Last
        {
            get
            {
                if (Wrappers.Count == 0)
                {
                    return null;
                }

                return
                    Wrappers[0].Report.SyncDate;
            }
        }

        public JournalWrapper()
        {
            Wrappers = new List<JournalItemWrapper>();
        }

        public void AddReverse(List<JournalItemWrapper> journalItems)
        {
            if (journalItems == null)
            {
                throw new ArgumentNullException("journalItems");
            }

            foreach (var ji in journalItems.OrderBy(j => j.SyncDate))
            {
                this.Wrappers.Insert(0, ji);
            }
        }
    }
}