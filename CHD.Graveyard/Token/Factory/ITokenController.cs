namespace CHD.Graveyard.Token.Factory
{
    public interface ITokenController : ITokenFactory
    {
        bool TryToReleaseToken(
            );
    }
}