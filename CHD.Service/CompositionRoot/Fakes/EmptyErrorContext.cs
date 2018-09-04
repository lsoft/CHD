using PerformanceTelemetry.ErrorContext;

namespace CHD.Service.CompositionRoot.Fakes
{
    internal sealed class EmptyErrorContext : IErrorContext
    {
        public void Dispose()
        {
            //nothing to do
        }
    }
}