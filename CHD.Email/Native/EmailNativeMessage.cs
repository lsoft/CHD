using System;
using CHD.Common.Native;
using MailKit;

namespace CHD.Email.Native
{
    public sealed class EmailNativeMessage : NativeMessage
    {
        public IMessageSummary Summary
        {
            get;
            private set;
        }

        public EmailNativeMessage(
            IMessageSummary summary
            ) : base(summary.Envelope.Subject)
        {
            if (summary == null)
            {
                throw new ArgumentNullException("summary");
            }

            Summary = summary;
        }
    }
}