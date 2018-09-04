using System;
using System.Collections.Generic;
using System.Linq;

namespace CHD.Settings.Controller
{
    public sealed class SettingRecord : ISettingRecord
    {
        private readonly List<string> _values;

        public string Name
        {
            get;
            private set;
        }


        public string Value
        {
            get
            {
                return
                    _values.FirstOrDefault();
            }
        }

        public IReadOnlyList<string> Values
        {
            get
            {
                return
                    _values;
            }
        }

        public bool AllowManyChildren
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

        public IReadOnlyList<string> PredefinedValues
        {
            get;
            private set;
        }

        public SettingRecord(
            string name,
            List<string> values,
            bool allowManyChildren,
            string comment,
            string preferredValue,
            List<string> predefinedValues
            )
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (preferredValue == null)
            {
                throw new ArgumentNullException("preferredValue");
            }
            if (predefinedValues == null)
            {
                throw new ArgumentNullException("predefinedValues");
            }

            Name = name;
            AllowManyChildren = allowManyChildren;
            _values = values;
            Comment = comment;
            PreferredValue = preferredValue;
            PredefinedValues = predefinedValues;
        }

        public void UpdateValues(IReadOnlyList<string> values)
        {
            _values.Clear();
            _values.AddRange(values);
        }
    }
}