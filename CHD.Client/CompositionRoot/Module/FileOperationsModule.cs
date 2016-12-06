using CHD.Client.FileOperation.Container;
using Ninject.Modules;

namespace CHD.Client.CompositionRoot.Module
{
    internal class FileOperationsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IActualFileOperationsContainer>()
                .To<ActualFileOperationsContainer>()
                .InSingletonScope()
                ;
        }
    }
}