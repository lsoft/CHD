using System;
using Ninject.Activation;
using Ninject.Parameters;
using Ninject.Planning.Targets;

namespace CHD.Ninject
{
    /// <summary>
    /// Overrides the injected value of a constructor argument.
    /// </summary>
    public sealed class TargetTypeConstructorArgument<T> : Parameter, IConstructorArgument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetTypeConstructorArgument{T}"/> class.
        /// </summary>
        /// <param name="argumentName">The argumentName of the argument to override.</param>
        /// <param name="value">The value to inject into the property.</param>
        public TargetTypeConstructorArgument(string argumentName, object value)
            : base(argumentName, value, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetTypeConstructorArgument{T}"/> class.
        /// </summary>
        /// <param name="argumentName">The argumentName of the argument to override.</param>
        /// <param name="valueCallback">The callback to invoke to get the value that should be injected.</param>
        public TargetTypeConstructorArgument(string argumentName, Func<IContext, object> valueCallback)
            : base(argumentName, valueCallback, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetTypeConstructorArgument{T}"/> class.
        /// </summary>
        /// <param name="argumentName">The argumentName of the argument to override.</param>
        /// <param name="valueCallback">The callback to invoke to get the value that should be injected.</param>
        public TargetTypeConstructorArgument(string argumentName, Func<IContext, ITarget, object> valueCallback)
            : base(argumentName, valueCallback, true)
        {
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

            var result = typeof(T) == target.Member.DeclaringType;

            return
                result;
        }
    }
}
