namespace CHD.Token
{
    public interface ITokenController : ITokenFactory
    {
        bool TryToReleaseToken(
            );
    }
}