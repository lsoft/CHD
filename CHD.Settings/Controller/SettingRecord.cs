using System;
using System.Collections.Generic;

namespace CHD.Settings.Controller
{
    public class SettingRecord : ISettingRecord
    {
        public string Name
        {
            get;
            private set;
        }


        public string Value
        {
            get;
            private set;
        }

        public string Comment
        {
            get;
            private set;
        }

        public string PreferredValue
        {
            get;
            private set;
        }

        public List<string> Values
        {
            get;
            private set;
        }

        public SettingRecord(
            string name,
            string value,
            string comment,
            string preferredValue,
            List<string> values
            )
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (preferredValue == null)
            {
                throw new ArgumentNullException("preferredValue");
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            Name = name;
            Value = value;
            Comment = comment;
            PreferredValue = preferredValue;
            Values = values;
        }

        public void UpdateValue(string value)
        {
            Value = value;
        }
    }
}