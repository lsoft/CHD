using System;
using System.Diagnostics;

namespace CHD.Common.Serializer
{
    [Serializable]
    [DebuggerDisplay("{VersionAtBorn} -> {VersionAtThisTime}")]
    public sealed class SerializationVersionProvider<T>
    {
        public int VersionAtBorn
        {
            get;
            private set;
        }

        public int VersionAtThisTime
        {
            get
            {
                var result = SerializationHelper.GetVersion<T>();

                return
                    result;
            }
        }

        public bool HadUpgraded
        {
            get
            {
                return
                    VersionAtBorn < VersionAtThisTime;
            }
        }

        public SerializationVersionProvider(
            )
        {
            VersionAtBorn = SerializationHelper.GetVersion<T>();
        }
    }
}