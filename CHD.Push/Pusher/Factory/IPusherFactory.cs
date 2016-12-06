using System.Xml;
using CHD.FileSystem.FileWrapper;
using CHD.Push.ActivityPool;

namespace CHD.Push.Pusher.Factory
{
    public interface IPusherFactory
    {
        IPusher Load(
            XmlNode source
            );

        IPusher CreatePusher(
            ActivityTypeEnum activity,
            IFileWrapper fileWrapper
            );
    }
}
