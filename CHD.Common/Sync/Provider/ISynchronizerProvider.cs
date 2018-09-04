namespace CHD.Common.Sync.Provider
{
    public interface ISynchronizerProvider
    {
        ISynchronizer CreateSynchronizer(
            );
    }
}