using CHD.Common.Native;
using CHD.Common.Saver;

namespace CHD.Common.Letter.Factory
{
    public interface ILetterFactory<TNativeMessage>
        where TNativeMessage : NativeMessage
    {
        ILetter<TNativeMessage> Create(
            TNativeMessage message
            );
    }
}