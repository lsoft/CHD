using Ninject.Activation;
using Ninject.Planning.Targets;

namespace CHD.Ninject
{
    public interface IMetadataArgumentProvider
    {
        object GetArgumentValue(
            IContext context,
            ITarget target
            );

        bool TryApplicable(
            IContext context,
            ITarget target
            );
    }
}