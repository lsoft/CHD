using System.Collections.Generic;

namespace CHD.Settings.Controller
{
    public interface ISettingRecordInner
    {
        string Name
        {
            get;
        }

        IReadOnlyList<string> Values
        {
            get;
        }


    }
}