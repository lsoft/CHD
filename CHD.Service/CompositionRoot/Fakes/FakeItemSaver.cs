using PerformanceTelemetry.Container.Saver.Item;
using PerformanceTelemetry.Record;

namespace CHD.Service.CompositionRoot.Fakes
{
    public sealed class FakeItemSaver : IItemSaver
    {

        public FakeItemSaver(
            )
        {
        }

        public void SaveItems(
            IPerformanceRecordData[] items,
            int itemCount
            )
        {
            //nothing to do
        }

        public void Commit()
        {
            //nothing to do
        }

        public void Dispose()
        {
            //nothing to do
        }
    }
}