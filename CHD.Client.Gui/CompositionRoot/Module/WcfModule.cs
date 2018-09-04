using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHD.Client.Gui.Wcf;
using Ninject;
using Ninject.Extensions.Factory;
using Ninject.Modules;

namespace CHD.Client.Gui.CompositionRoot.Module
{
    public sealed class WcfModule : NinjectModule
    {
        public WcfModule(
            )
        {
        }

        public override void Load()
        {
            Bind<IBindingProvider>()
                .To<DataBindingProvider>()
                ;

            //Bind<IDataChannel>()
            //    .To<WcfDataChannel>()
            //    ;

            Bind<IDataChannelFactory>()
                .To<DataChannelFactory>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "enpointAddress",
                    c => c.Kernel.Get<AppSettings>().DataChannelEndpoint
                    )
                ;
        }
    }
}
