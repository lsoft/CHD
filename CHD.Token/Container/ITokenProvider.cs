namespace CHD.Token.Container
{
    public interface ITokenProvider
    {
        bool IsTokenTaken
        {
            get;
        }

        event TokenStatusDelegate TokenStatusEvent;
    }
}