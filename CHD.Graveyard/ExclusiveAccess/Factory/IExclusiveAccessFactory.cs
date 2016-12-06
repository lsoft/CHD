using System;

namespace CHD.Graveyard.ExclusiveAccess.Factory
{
    public interface IExclusiveAccessFactory
    {
        IExclusiveAccess GetExclusiveAccess(
            Action closeAction
            );
    }
}