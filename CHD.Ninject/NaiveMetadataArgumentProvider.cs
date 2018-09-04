using System;
using System.Linq;
using Ninject.Activation;
using Ninject.Planning.Targets;

namespace CHD.Ninject
{
    public sealed class NaiveMetadataArgumentProvider : IMetadataArgumentProvider
    {
        private readonly string _metadataName;
        private readonly MetadataArgumentPair[] _metadataPairs;

        public NaiveMetadataArgumentProvider(
            string metadataName,
            params MetadataArgumentPair[] metadataPairs
            )
        {
            if (metadataName == null)
            {
                throw new ArgumentNullException("metadataName");
            }
            if (metadataPairs == null)
            {
                throw new ArgumentNullException("metadataPairs");
            }

            _metadataName = metadataName;
            _metadataPairs = metadataPairs;
        }

        public object GetArgumentValue(IContext context, ITarget target)
        {
            var foundPair = FindPair(context);

            if (foundPair == null)
            {
                return
                    null;
            }

            return
                foundPair.Argument;
        }

        public bool TryApplicable(IContext context, ITarget target)
        {
            var foundPair = FindPair(context);

            return
                foundPair != null;
        }

        private MetadataArgumentPair FindPair(IContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var realValue = context.Binding.BindingConfiguration.Metadata.Get<string>(_metadataName);

            if (realValue == null)
            {
                return null;
            }

            var foundPair = _metadataPairs.FirstOrDefault(pair => pair.TryApplicable(realValue));

            if (foundPair == null)
            {
                return null;
            }

            return foundPair;
        }
    }
}