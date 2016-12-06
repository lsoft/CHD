namespace CHD.Push.ActivityPool
{
    public interface ITimeoutActivityPool : IActivityPool
    {
        void AsyncStart();

        void SyncStop();
    }
}