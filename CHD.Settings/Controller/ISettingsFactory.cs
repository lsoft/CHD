using CHD.Common.Crypto;

namespace CHD.Settings.Controller
{
    public interface ISettingsFactory
    {
        ISettings LoadSettings(
            string filePath,
            ICrypto crypto
            );
    }
}