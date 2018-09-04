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
                case ModeEnum.Disk:
                    return
                        "��������� ����";
                case ModeEnum.Email:
                    return
                        "�����";
                case ModeEnum.CloudMailru:
                    return
                        "������ Mail.Ru";
                default:
                    throw new ArgumentOutOfRangeException("mode");
            }
        }
    }
}