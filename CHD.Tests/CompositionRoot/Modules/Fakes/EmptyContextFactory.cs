using PerformanceTelemetry.ErrorContext;

namespace CHD.Tests.CompositionRoot.Modules.Fakes
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