namespace CHD.Service.Runner
{
    public interface IScheduledScannerRunner
    {
        void AsyncStart();

        void SyncStop();
    }
}