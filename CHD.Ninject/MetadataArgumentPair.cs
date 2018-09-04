using System;

namespace CHD.Ninject
{
    public sealed class MetadataArgumentPair
    {
        public string MetadataValue
        {
            get;
            private set;
        }

        public object Argument
        {
            get;
            private set;
        }

        public MetadataArgumentPair(string metadataValue, object argument)
        {
            MetadataValue = metadataValue;
            Argument = argument;
        }

        public bool TryApplicable(
            IComparable metadataValue
            )
        {
            var result = metadataValue.CompareTo(MetadataValue);

            return
                result == 0;
        }
    }
}