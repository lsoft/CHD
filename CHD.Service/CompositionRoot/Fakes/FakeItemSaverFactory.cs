using PerformanceTelemetry.Container.Saver.Item;

namespace CHD.Service.CompositionRoot.Fakes
{
    public sealed class FakeItemSaverFactory : IItemSaverFactory
    {
        public FakeItemSaverFactory(
            )
        {
        }

        public IItemSaver CreateItemSaver()
        {
            return
                new FakeItemSaver(
                    );
        }
    }
}