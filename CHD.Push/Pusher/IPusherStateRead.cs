namespace CHD.Push.Pusher
{
    public interface IPusherStateRead
    {
        float Progress
        {
            get;
        }

        bool IsWorking
        {
            get;
        }

        bool IsCancelled
        {
            get;
        }

        bool IsCompletedSuccessfully
        {
            get;
        }

        bool IsDead
        {
            get;
        }
    }
}