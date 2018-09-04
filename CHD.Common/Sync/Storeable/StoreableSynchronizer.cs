using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHD.Common.Sync.Report;
using CHD.Common.Sync.Report.Journal;

namespace CHD.Common.Sync.Storeable
{
    public sealed class StoreableSynchronizer : ISynchronizer
    {
        private readonly ISynchronizer _synchronizer;
        private readonly ISyncJournal _container;

        public StoreableSynchronizer(
            ISynchronizer synchronizer,
            ISyncJournal container
            )
        {
            if (synchronizer == null)
            {
                throw new ArgumentNullException("synchronizer");
            }
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            _synchronizer = synchronizer;
            _container = container;
        }

        public SyncReport Sync()
        {
            var report = _synchronizer.Sync();

            _container.Add(report);

            return
                report;
        }
    }
}
