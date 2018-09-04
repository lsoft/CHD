using System;
using Ninject.Activation;
using Ninject.Parameters;
using Ninject.Planning.Targets;

namespace CHD.Ninject
{
    /// <summary>
    /// Overrides the injected value of a constructor argument.
    /// </summary>
    public sealed class MetadataConstructorArgument : Parameter, IConstructorArgument
    {
        private readonly IMetadataArgumentProvider _metadataArgumentProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataConstructorArgument"/> class.
        /// </summary>
        /// <param name="argumentName">The argumentName of the argument to override.</param>
        /// <param name="value">The value to inject into the property.</param>
        public MetadataConstructorArgument(string argumentName, object value)
            : base(argumentName, value, true)
        {
            throw new InvalidOperationException("Not applicable");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataConstructorArgument"/> class.
        /// </summary>
        /// <param name="argumentName">The argumentName of the argument to override.</param>
        /// <param name="metadataArgumentProvider">Metadata checker</param>
        public MetadataConstructorArgument(string argumentName, IMetadataArgumentProvider metadataArgumentProvider)
            : base(argumentName, metadataArgumentProvider.GetArgumentValue, true)
        {
            if (metadataArgumentProvider == null)
            {
                throw new ArgumentNullException("metadataArgumentProvider");
            }

            _metadataArgumentProvider = metadataArgumentProvider;
        }

        /// <summary>
        /// Determines if the parameter applies to the given target.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>
        /// True if the parameter applies in the specified context to the specified target.
        /// </returns>
        /// <remarks>
        /// Only one parameter may return true.
        /// </remarks>
        public bool AppliesToTarget(IContext context, ITarget target)
        {
            var argeq = string.Equals(this.Name, target.Name);

            if (!argeq)
            {
                return false;
            }

            var metadataeq = _metadataArgumentProvider.TryApplicable(context, target);

            if (!metadataeq)
            {
                return false;
            }

            return
                true;
        }
    }
}