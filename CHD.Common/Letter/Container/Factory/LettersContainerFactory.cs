using System;
using System.Collections.Generic;
using CHD.Common.Native;
using CHD.Common.Saver;

namespace CHD.Common.Letter.Container.Factory
{
    public sealed class LettersContainerFactory<TNativeMessage> : ILettersContainerFactory<TNativeMessage>
        where TNativeMessage : NativeMessage
    {
        private readonly IDisorderLogger _logger;

        public LettersContainerFactory(
            IDisorderLogger logger
            )
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _logger = logger;
        }

        public ILettersContainer<TNativeMessage> Create(
            List<ILetter<TNativeMessage>> letters
            )
        {
            if (letters == null)
            {
                throw new ArgumentNullException("letters");
            }

            var result = new LettersContainer<TNativeMessage>(
                letters
                );

            return result;
        }
    }
}