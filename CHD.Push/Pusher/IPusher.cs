using System.Xml;
using CHD.FileSystem.FileWrapper;
using CHD.Push.ActivityPool;

namespace CHD.Push.Pusher
{
    public interface IPusher : IPusherState
    {
        IFileWrapper FileWrapper
        {
            get;
        }

        ActivityTypeEnum Type
        {
            get;
        }

        void Serialize(
            XmlNode target
            );
    }
}