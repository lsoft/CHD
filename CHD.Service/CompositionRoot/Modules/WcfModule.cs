using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using CHD.Service.Wcf;
using Ninject.Modules;

namespace CHD.Service.CompositionRoot.Modules
{
    internal sealed class WcfModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IServiceBehavior>()
                .To<WcfDataBehaviour>()
                ;

            Bind<IWcfListener>()
                .To<WcfDataListener>()
                .InSingletonScope()
                ;
        }
    }
}
