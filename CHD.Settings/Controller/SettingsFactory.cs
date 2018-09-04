using System;

namespace CHD.Settings.Controller
{
    public sealed class SettingsFactory : ISettingsFactory
    {
        public SettingsFactory(
            )
        {
        }

        public ISettings LoadSettings(
            string filePath 
            )
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            var result = new Settings(
                filePath
                );

            return
                result;
        }
    }
}