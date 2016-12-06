using System;
using System.Collections.Generic;
using CHD.Common.Crypto;

namespace CHD.Settings.Controller
{
    public interface ISettings
    {
        IReadOnlyCollection<ISettingRecord> Records
        {
            get;
        }

        void Update(
            ISettingRecordInner updated
            );

        void Rewrite(
            ICrypto crypto
            );

        void Export(
            IDictionary<string, Action<string>> actions
            );

    }
}