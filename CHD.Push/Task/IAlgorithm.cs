using System.Xml;
using CHD.Dynamic.Scheduler.Task;
using CHD.Push.ActivityPool;
using CHD.Push.Pusher;

namespace CHD.Push.Task
{
    public interface IAlgorithm : ITask
    {
        ActivityTypeEnum Type
        {
            get;
        }

        IPusher Pusher
        {
            get;
        }

        void Serialize(
            XmlNode target
            );
    }
}