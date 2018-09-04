using System;
using CHD.Common.KeyValueContainer;

namespace CHD.Token.Container
{
    public sealed class KeyValueTokenContainer : ITokenContainer
    {
        private const string TokenVariableName = "TokenCurrentStatus";

        private readonly IKeyValueContainer _keyValueContainer;

        public bool IsTokenTaken
        {
            get;
            private set;
        }

        public event TokenStatusDelegate TokenStatusEvent;

        public KeyValueTokenContainer(
            IKeyValueContainer keyValueContainer
            )
        {
            if (keyValueContainer == null)
            {
                throw new ArgumentNullException("keyValueContainer");
            }
            _keyValueContainer = keyValueContainer;

            Read();
        }


        public void UpdateTokenTakenStatus(bool taken)
        {
            if (IsTokenTaken == taken)
            {
                return;
            }

            IsTokenTaken = taken;

            _keyValueContainer.AddOrUpdate(TokenVariableName, taken.ToString());

            OnTokenStatus(taken);
        }


        private void Read()
        {
            string tokenStatus;
            if (_keyValueContainer.TryGet(TokenVariableName, out tokenStatus))
            {
                IsTokenTaken = bool.Parse(tokenStatus);
            }
        }


        protected void OnTokenStatus(bool currenttokenstatus)
        {
            TokenStatusDelegate handler = TokenStatusEvent;
            if (handler != null)
            {
                handler(currenttokenstatus);
            }
        }

    }
}