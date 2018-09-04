using System;
using CHD.Tests.CompositionRoot.Modules.Fakes.Stat;
using PerformanceTelemetry.Container.Saver.Item;
using PerformanceTelemetry.Record;

namespace CHD.Tests.CompositionRoot.Modules.Fakes
{
    public sealed class StatSaver : IItemSaver
    {
        private readonly StatRecordContainer _recordContainer;

        public StatSaver(
            StatRecordContainer recordContainer
            )
        {
            if (recordContainer == null)
            {
                throw new ArgumentNullException("recordContainer");
            }

            _recordContainer = recordContainer;
        }

        public void SaveItems(
            IPerformanceRecordData[] items,
            int itemCount
            )
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            for (var cc = 0; cc < itemCount; cc++)
            {
                var record = items[cc];

                if (record.Exception == null)
                {
                    //строим статистику только по вызовам без эксцепшенов
                    _recordContainer.AddRecord(record);
                }
            }
        }

        public void Commit()
        {
            //nothing to do
        }

        public void Dispose()
        {
            //nothing to do
        }

        //private void SaveItem(
        //    IPerformanceRecordData item
        //    )
        //{
        //    if (item == null)
        //    {
        //        throw new ArgumentNullException("item");
        //    }

        //    var children = item.GetChildren();

        //    Debug.WriteLine(
        //        "[{0} - {1}] {2}.{3} || {4} || Children count = {5}",
        //        item.StartTime.ToString("yyyyMMdd HH:mm:ss.fff"),
        //        item.StartTime.AddSeconds(item.TimeInterval).ToString("yyyyMMdd HH:mm:ss.fff"),
        //        item.ClassName,
        //        item.MethodName,
        //        item.Exception != null ? item.Exception.Message : "-= NO EXCEPTION =-",
        //        children.Count
        //        );
        //}

    }
}