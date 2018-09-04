using System;
using System.Threading;
using CHD.Common.Native;

namespace CHD.Common.Letter
{
    public sealed class SendableWrapper<TSendableMessage> : IDisposable
        where TSendableMessage : SendableMessage
    {
        private readonly Action _disposeAction;

        private long _disposed = 0L;

        public TSendableMessage Message
        {
            get;
            private set;
        }

        public SendableWrapper(
            TSendableMessage message,
            Action disposeAction
            )
        {
            _disposeAction = disposeAction;
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (disposeAction == null)
            {
                throw new ArgumentNullException("disposeAction");
            }

            Message = message;
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1L) == 0L)
            {
                _disposeAction();
            }
        }
    }
}