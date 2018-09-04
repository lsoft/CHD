using System.Runtime.Serialization;

namespace CHD.WcfChannel.Common
{
    [DataContract]
    public sealed class WcfCommonInfo
    {
        [DataMember]
        public string WatchFolder
        {
            get;
            private set;
        }

        public WcfCommonInfo(string watchFolder)
        {
            WatchFolder = watchFolder;
        }
    }
}