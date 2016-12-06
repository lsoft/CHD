using System;
using System.Threading;
using CHD.Common.Logger;
using CHD.Graveyard.ExclusiveAccess;
using CHD.Graveyard.Graveyard;
using CHD.Push.ActivityPool;
using CHD.Push.Pusher;

namespace CHD.Push.Task
{
    internal class DeleteFileAlgorithm : BaseAlgorithm
    {
        private readonly IGraveyard _graveyard;
        
        private IExclusiveAccess _exclusiveAccess;

        public DeleteFileAlgorithm(
            IPusher pusher,
            IGraveyard graveyard,
            Guid taskGuid,
            int pushTimeoutAfterFailureMsec,
            IDisorderLogger logger
            )
            : base(pusher, taskGuid, pushTimeoutAfterFailureMsec, logger)
        {
            if (graveyard == null)
            {
                throw new ArgumentNullException("graveyard");
            }

            _graveyard = graveyard;
        }


        protected override void DoLocalPreparation()
        {
            //nothing to do
        }

        protected override void DoRemotePreparation(
            out RemotePreparationResultEnum result
            )
        {
            if (!_graveyard.TryGetExclusiveAccess(out _exclusiveAccess))
            {
                result = RemotePreparationResultEnum.ShouldRepeat;
                return;
            }

            if (_exclusiveAccess.ContainsTransaction(this.TaskGuid))
            {
                result = RemotePreparationResultEnum.AlreadyCommitted;
                return;
            }
            
            result = RemotePreparationResultEnum.AllowToContinue;
        }

        protected override float DoRemoteIteration(
            )
        {
            _exclusiveAccess.DeleteRemoteFile(
                TaskGuid,
                _pusher.FileWrapper
                );

            return
                1f;
        }

        protected override void SafelyCloseRemoteInfrastructure()
        {
            SafelyCloseExclusiveAccess();
        }

        protected override void SafelyCloseLocalInfrastructure()
        {
            //nothing to do
        }

        private void SafelyCloseExclusiveAccess()
        {
            var exclusiveAccess = Interlocked.Exchange(ref _exclusiveAccess, null);
            if (exclusiveAccess != null)
            {
                try
                {
                    exclusiveAccess.Close();

                }
                catch (Exception excp)
                {
                    _logger.LogException(excp);
                }
            }
        }
    }
}