namespace CHD.Push.Pusher
{
    public interface IPusherState : IPusherStateRead
    {
        void SetProgress(float progress);

        void SetIsWorking(bool isWorking);

        void SetIsCancelled(bool isCancelled);

        void SetIsCompleted(bool isCompletedSuccessfully);
    }
}