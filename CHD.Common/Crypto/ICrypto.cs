namespace CHD.Common.Crypto
{
    public interface ICrypto
    {
        void LoadKey(
            byte[] newKey
            );




        byte[] EncodeBuffer(
            byte[] buffer
            );

        byte[] DecodeBuffer(
            byte[] buffer
            );





        ulong EncodeValue(
            ulong value
            );

        ulong DecodeValue(
            ulong value
            );
    }
}