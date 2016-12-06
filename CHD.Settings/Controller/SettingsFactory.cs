using System;
using CHD.Common.Crypto;

namespace CHD.Settings.Controller
{
    public class SettingsFactory : ISettingsFactory
    {
        public SettingsFactory(
            )
        {
        }

        public ISettings LoadSettings(
            string filePath, 
            ICrypto crypto
            )
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }
            if (crypto == null)
            {
                throw new ArgumentNullException("crypto");
            }

            var result = new Settings(
                filePath,
                crypto
                );

            return
                result;
        }
    }
}