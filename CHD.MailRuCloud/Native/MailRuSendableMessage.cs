using System;
using CHD.Common.Native;

namespace CHD.MailRuCloud.Native
{
    public sealed class MailRuSendableMessage : SendableMessage
    {
        public string Name
        {
            get;
            private set;
        }

        public byte[] Data
        {
            get;
            private set;
        }

        public MailRuSendableMessage(
            string name,
            byte[] data
            )
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            Name = name;
            Data = data;
        }
    }
}