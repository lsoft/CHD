using System.Collections.Generic;

namespace CHD.Settings.Controller
{
    public interface ISettingRecord : ISettingRecordInner
    {
        bool AllowManyChildren
        {
            get;
        }

        string Comment
        {
            get;
        }

        string PreferredValue
        {
            get;
        }

        string Value
        {
            get;
        }

        IReadOnlyList<string> PredefinedValues
        {
            get;
        }

        void UpdateValues(
            IReadOnlyList<string> values
            );
    }
}