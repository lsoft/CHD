using CHD.Client.Gui.DataFlow;
using CHD.Client.Gui.DataFlow.Retriever;
using Ninject.Modules;

namespace CHD.Client.Gui.CompositionRoot.Module
{
    public sealed class CommonComponentsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDataProvider, IDataContainer>()
                .To<DataContainer>()
                .InSingletonScope()
                ;

            Bind<IDataRetriever>()
                .To<PeriodicallyDataRetriever>()
                .InSingletonScope()
                ;
        }
    }
}