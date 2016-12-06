using System;

namespace CHD.Settings.Mode
{
    public static class ModeEnumHelper
    {
        public static string GetDescription(
            ModeEnum mode
            )
        {
            switch (mode)
            {
                case ModeEnum.Local:
                    return
                        "Локальный";
                case ModeEnum.Email:
                    return
                        "Емейл";
                case ModeEnum.CloudMailru:
                    return
                        "Облако Mail.Ru";
                default:
                    throw new ArgumentOutOfRangeException("mode");
            }
        }
    }
}