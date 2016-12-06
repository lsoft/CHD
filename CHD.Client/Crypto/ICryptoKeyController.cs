namespace CHD.Client.Crypto
{
    public interface ICryptoKeyController : ICryptoKeyContainer
    {
        void SetKey(byte[] cryptoKey);
    }
}