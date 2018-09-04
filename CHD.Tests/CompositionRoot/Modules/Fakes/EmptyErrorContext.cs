using PerformanceTelemetry.ErrorContext;

namespace CHD.Tests.CompositionRoot.Modules.Fakes
{
    internal sealed class EmptyErrorContext : IErrorContext
    {
        public void Dispose()
        {
            //nothing to do
        }
    }
}