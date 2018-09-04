using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHD.Client.Gui.ViewModel.Main;
using CHD.Client.Gui.ViewModel.Main.Wrapper.Common;
using CHD.Client.Gui.ViewModel.Main.Wrapper.Journal;
using CHD.Client.Gui.ViewModel.Main.Wrapper.Sync;
using CHD.WcfChannel.Journal;

namespace CHD.Client.Gui.DataFlow
{
    public sealed class DataContainer : IDataContainer
    {
        public CommonWrapper CommonData
        {
            get;
            private set;
        }

        public SyncWrapper SyncData
        {
            get;
            private set;
        }

        public JournalWrapper JournalData
        {
            get;
            private set;
        }

        public event CommonDataChangedDelegate CommonDataChangedEvent;

        public event SyncDataChangedDelegate SyncDataChangedEvent;

        public event JournalDataChangedDelegate JournalDataChangedEvent;

        public DataContainer()
        {
            CommonData = new CommonWrapper();
            SyncData = new SyncWrapper();
            JournalData = new JournalWrapper();
        }

        public bool Set(
            CommonWrapper commonData
            )
        {
            if (commonData == null)
            {
                throw new ArgumentNullException("commonData");
            }

            var changesExists = CommonData.ChangesExists(commonData);

            if (changesExists)
            {
                CommonData = commonData;
                OnCommonDataChanged();
            }

            return
                changesExists;
        }

        public bool Set(
            SyncWrapper syncData
            )
        {
            if (syncData == null)
            {
                throw new ArgumentNullException("syncData");
            }

            var changesExists = SyncData.ChangesExists(syncData);

            if (changesExists)
            {
                SyncData = syncData;
                OnSyncDataChanged();
            }

            return
                changesExists;
        }

        public bool Add(
            List<WcfHistorySyncReport> journalItems
            )
        {
            if (journalItems == null)
            {
                throw new ArgumentNullException("journalItems");
            }

            if (journalItems.Count == 0)
            {
                return false;
            }

            var converted = journalItems
                .Select(j => new JournalItemWrapper(j))
                .ToList()
                ;

            JournalData.AddReverse(converted);

            OnJournalDataChanged(converted);

            return true;
        }

        private void OnCommonDataChanged()
        {
            CommonDataChangedDelegate handler = CommonDataChangedEvent;
            if (handler != null)
            {
                handler();
            }
        }

        private void OnSyncDataChanged()
        {
            SyncDataChangedDelegate handler = SyncDataChangedEvent;
            if (handler != null)
            {
                handler();
            }
        }

        private void OnJournalDataChanged(List<JournalItemWrapper> newJournalItems)
        {
            JournalDataChangedDelegate handler = JournalDataChangedEvent;
            if (handler != null)
            {
                handler(newJournalItems);
            }
        }

    }
}
