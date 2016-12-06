using System.Collections.Generic;

namespace CHD.Settings.Controller
{
    public interface ISettingRecord : ISettingRecordInner
    {
        string Comment
        {
            get;
        }

        string PreferredValue
        {
            get;
        }

        List<string> Values
        {
            get;
        }

        void UpdateValue(
            string value
            );
    }
}