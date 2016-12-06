namespace CHD.Graveyard.Token.Factory
{
    public interface ITokenFactory
    {
        bool TryToObtainToken(
            out IToken token
            );
    }
}