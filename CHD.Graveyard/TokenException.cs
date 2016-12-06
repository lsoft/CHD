using System;
using System.Runtime.Serialization;

namespace CHD.Graveyard
{
    public class TokenException : Exception
    {
        public TokenException(string message, TokenActionEnum failedAction) : base(message)
        {
            FailedAction = failedAction;
        }

        public TokenException(string message, Exception innerException, TokenActionEnum failedAction) : base(message, innerException)
        {
            FailedAction = failedAction;
        }

        protected TokenException(SerializationInfo info, StreamingContext context, TokenActionEnum failedAction) : base(info, context)
        {
            FailedAction = failedAction;
        }

        public TokenActionEnum FailedAction
        {
            get;
            private set;
        }

        public TokenException(TokenActionEnum failedAction)
        {
            FailedAction = failedAction;
        }
    }
}
