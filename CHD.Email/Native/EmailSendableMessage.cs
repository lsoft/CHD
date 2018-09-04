using System;
using CHD.Common.Native;
using MimeKit;

namespace CHD.Email.Native
{
    public sealed class EmailSendableMessage : SendableMessage
    {
        public MimeMessage Message
        {
            get;
            private set;
        }

        public EmailSendableMessage(
            MimeMessage message
            )
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            Message = message;
        }
    }
}