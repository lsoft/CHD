using System;
using CHD.Common.Native;
using MailRu.Cloud.WebApi;

namespace CHD.MailRuCloud.Native
{
    public sealed class MailRuNativeMessage : NativeMessage
    {
        public File File
        {
            get;
            private set;
        }

        public MailRuNativeMessage(File file)
            : base(file.Name)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            File = file;
        }
    }
}