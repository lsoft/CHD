using CHD.Graveyard.RemoteFileState;

namespace CHD.Pull.Components.Factory
{
    public interface IExecutionUnitFactory
    {
        IExecutionUnit Create(
            IRemoteFileState fileState
            );
    }
}
