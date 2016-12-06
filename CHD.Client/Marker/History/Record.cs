using System;
using System.IO;
using CHD.Common;

namespace CHD.Client.Marker.History
{
    internal class Record
    {
        public DateTime Time
        {
            get;
            private set;
        }

        public bool Taken
        {
            get;
            private set;
        }

        public bool Success
        {
            get;
            private set;
        }

        public string ExceptionInformation
        {
            get;
            private set;
        }

        public Record(DateTime time, bool taken)
        {
            Time = time;
            Taken = taken;
            Success = true;
            ExceptionInformation = string.Empty;
        }

        public Record(DateTime time, bool taken, string exceptionInformation)
        {
            if (exceptionInformation == null)
            {
                throw new ArgumentNullException("exceptionInformation");
            }

            Time = time;
            Taken = taken;
            Success = false;
            ExceptionInformation = exceptionInformation;
        }

        internal Record(DateTime time, bool taken, bool success, string exceptionInformation)
        {
            if (exceptionInformation == null)
            {
                throw new ArgumentNullException("exceptionInformation");
            }

            Time = time;
            Taken = taken;
            Success = success;
            ExceptionInformation = exceptionInformation;
        }

        public string Serialize(
            )
        {
            return
                string.Format(
                    "{1}{0}{2}{0}{3}{0}{4}",
                    Path.DirectorySeparatorChar,
                    Time.Ticks,
                    Taken,
                    Success,
                    Base64Helper.EncodeToString(ExceptionInformation)
                    );
        }

        public static Record Deserialize(
            string s
            )
        {
            var parts  = s.Split(Path.DirectorySeparatorChar);

            var time = new DateTime(long.Parse(parts[0]));
            var taken = bool.Parse(parts[1]);
            var success = bool.Parse(parts[2]);
            var ei = Base64Helper.DecodeFromString(parts[3]);

            return 
                new Record(
                    time,
                    taken,
                    success,
                    ei
                    );
        }
    }
}
