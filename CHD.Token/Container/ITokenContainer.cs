namespace CHD.Token.Container
{
    public interface ITokenContainer : ITokenProvider
    {
        void UpdateTokenTakenStatus(bool taken);
    }
}