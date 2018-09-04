using System.Collections.Generic;
using CHD.Common.Native;
using CHD.Common.Saver;

namespace CHD.Common.Letter.Container.Factory
{
    public interface ILettersContainerFactory<TNativeMessage>
        where TNativeMessage : NativeMessage
    {
        ILettersContainer<TNativeMessage> Create(
            List<ILetter<TNativeMessage>> letters
            );
    }
}