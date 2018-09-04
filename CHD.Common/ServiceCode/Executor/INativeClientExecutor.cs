using System;
using CHD.Common.Native;
using CHD.Common.Saver;

namespace CHD.Common.ServiceCode.Executor
{
    public interface INativeClientExecutor<TNativeMessage, TSendableMessage>
        where TNativeMessage : NativeMessage
        where TSendableMessage : SendableMessage
    {
        void Execute(
            Action<INativeClientEx<TNativeMessage, TSendableMessage>> executeAction
            );

        T Execute<T>(
            Func<INativeClientEx<TNativeMessage, TSendableMessage>, T> executeAction
            );
    }
}