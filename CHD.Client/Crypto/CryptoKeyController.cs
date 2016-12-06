namespace CHD.Client.Crypto
{
    public class CryptoKeyController : ICryptoKeyController
    {
        public byte[] CryptoKey
        {
            get;
            private set;
        }

        public void SetKey(byte[] cryptoKey)
        {
            CryptoKey = cryptoKey;
        }
    }
}