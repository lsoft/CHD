using PerformanceTelemetry.ErrorContext;

namespace CHD.Service.CompositionRoot.Fakes
{
    internal sealed class EmptyContextFactory : IErrorContextFactory
    {
        public IErrorContext OpenContext()
        {
            return
                new EmptyErrorContext();
        }
    }
}