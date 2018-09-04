using System;
using CHD.Tests.CompositionRoot.Modules.Fakes.Stat;
using PerformanceTelemetry.Container.Saver.Item;

namespace CHD.Tests.CompositionRoot.Modules.Fakes
{
    public sealed class StatSaverFactory : IItemSaverFactory
    {
        private readonly StatRecordContainer _recordContainer;

        public StatSaverFactory(
            StatRecordContainer recordContainer
            )
        {
            if (recordContainer == null)
            {
                throw new ArgumentNullException("recordContainer");
            }
            _recordContainer = recordContainer;
        }

        public IItemSaver CreateItemSaver()
        {
            return
                new StatSaver(
                    _recordContainer
                    );
        }
    }
}