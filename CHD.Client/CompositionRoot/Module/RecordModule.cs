using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHD.Client.Marker.History;
using Ninject;
using Ninject.Modules;

namespace CHD.Client.CompositionRoot.Module
{
    internal class RecordModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IRecordContainer>()
                .To<RecordContainer>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "filepath",
                    c => c.Kernel.Get<Settings.MainSettings>().MarkerHistory
                    )
                ;
        }
    }
}
