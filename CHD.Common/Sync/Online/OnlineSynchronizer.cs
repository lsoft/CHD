using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHD.Common.OnlineStatus.Sync;
using CHD.Common.Sync.Report;

namespace CHD.Common.Sync.Online
{
    public sealed class OnlineSynchronizer : ISynchronizer
    {
        private readonly ISynchronizer _synchronizer;
        private readonly ISyncOnlineStatus _onlineStatus;

        public OnlineSynchronizer(
            ISynchronizer synchronizer,
            ISyncOnlineStatus onlineStatus
            )
        {
            if (synchronizer == null)
            {
                throw new ArgumentNullException("synchronizer");
            }
            if (onlineStatus == null)
            {
                throw new ArgumentNullException("onlineStatus");
            }

            _synchronizer = synchronizer;
            _onlineStatus = onlineStatus;
        }

        public SyncReport Sync()
        {
            try
            {
                _onlineStatus.Start();

                var result = _synchronizer.Sync();

                _onlineStatus.EndWithSuccess(result);

                return
                    result;
            }
            catch
            {
                _onlineStatus.EndWithError();
                throw;
            }
        }
    }
}
