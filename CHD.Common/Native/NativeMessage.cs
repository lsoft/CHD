using System;

namespace CHD.Common.Native
{
    public class NativeMessage
    {
        public string Subject
        {
            get;
            private set;
        }

        public NativeMessage(string subject)
        {
            if (subject == null)
            {
                throw new ArgumentNullException("subject");
            }

            Subject = subject;
        }
    }
}