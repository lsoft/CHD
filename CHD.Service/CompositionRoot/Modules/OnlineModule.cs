using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHD.Common.OnlineStatus.Diff.Apply;
using CHD.Common.OnlineStatus.Diff.Apply.Operation;
using CHD.Common.OnlineStatus.Diff.Build;
using CHD.Common.OnlineStatus.Sync;
using Ninject.Modules;

namespace CHD.Service.CompositionRoot.Modules
{
    internal sealed class OnlineModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ISyncOnlineStatus, ISyncOnlineReport>()
                .To<SyncOnline>()
                .InSingletonScope()
                ;

            Bind<IDiffBuildOnlineReport, IDiffBuildOnlineStatus>()
                .To<DiffBuildOnline>()
                .InSingletonScope()
                ;

            Bind<IDiffApplyOnlineReport, IDiffApplyOnlineStatus, IOperationOnlineStatus>()
                .To<DiffApplyOnline>()
                .InSingletonScope()
                ;

            //Bind<>()
            //    .To<>()
            //    .InSingletonScope()
            //    ;

            //Bind<>()
            //    .To<>()
            //    .InSingletonScope()
            //    ;
        }
    }
}
