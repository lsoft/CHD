using System;
using CHD.Common.Native;
using CHD.Common.PathComparer;
using CHD.Common.Saver;
using CHD.Common.ServiceCode.Executor;

namespace CHD.Common.Letter.Factory
{
    public sealed class LetterFactory<TNativeMessage, TSendableMessage> : ILetterFactory<TNativeMessage>
        where TNativeMessage : NativeMessage
        where TSendableMessage : SendableMessage
    {
        private readonly IPathComparerProvider _pathComparerProvider;
        private readonly INativeClientExecutor<TNativeMessage, TSendableMessage> _executor;

        public LetterFactory(
            IPathComparerProvider pathComparerProvider,
            INativeClientExecutor<TNativeMessage, TSendableMessage> executor
            )
        {
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }
            if (executor == null)
            {
                throw new ArgumentNullException("executor");
            }
            _pathComparerProvider = pathComparerProvider;
            _executor = executor;
        }

        public ILetter<TNativeMessage> Create(
            TNativeMessage message
            )
        {
            var result = new Letter<TNativeMessage, TSendableMessage>(
                _pathComparerProvider,
                _executor,
                message
                );

            return
                result;
        }
    }
}