namespace CHD.Token
{
    public interface ITokenFactory
    {
        bool TryToObtainToken(
            out IToken token
            );
    }
}