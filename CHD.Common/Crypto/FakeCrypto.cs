using System;

namespace CHD.Common.Crypto
{
    public sealed class FakeCrypto : ICrypto
    {
        public void LoadKey(byte[] newKey)
        {
            //nothing to do
        }

        public byte[] EncodeBuffer(byte[] buffer)
        {
            return
                buffer;
        }

        public byte[] DecodeBuffer(byte[] buffer)
        {
            return
                buffer;
        }

        public ulong EncodeValue(ulong value)
        {
            return
                value;
        }

        public ulong DecodeValue(ulong value)
        {
            return
                value;
        }
    }
}