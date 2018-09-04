using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CHD.Settings.Controller;
using CHD.Wpf;

namespace CHD.Installer.ViewModel.Edit
{
    public sealed class Record : ISettingRecordInner
    {
        private readonly ISettingRecord _sr;
        private readonly List<StringWrapper> _values;

        public string Name
        {
            get;
            private set;
        }

        public ObservableCollection2<StringWrapper> EditableValues
        {
            get;
            private set;
        }

        public IReadOnlyList<string> Values
        {
            get
            {
                return
                    _values.ConvertAll(j => j.Value);
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

        public ObservableCollection2<Option> AvailableValues
        {
            get;
            private set;
        }

        public ICommand AddCommand
        {
            get;
            private set;
        }

        public ICommand DeleteCommand
        {
            get;
            private set;
        }

        public Record(
            ISettingRecord sr
            )
        {
            if (sr == null)
            {
                throw new ArgumentNullException("sr");
            }

            _sr = sr;

            EditableValues = new ObservableCollection2<StringWrapper>();
            _values = new List<StringWrapper>();

            Name = sr.Name;
            AllowManyChildren = sr.AllowManyChildren;
            Comment = sr.Comment;


            var converted = sr.Values.Select(j => new StringWrapper(j)).ToList();
            _values.AddRange(converted);
            EditableValues.AddRange(converted);

            AvailableValues = new ObservableCollection2<Option>();

            ProcessAvailableCollection();

            AddCommand = new RelayCommand(
                arg =>
                {
                    var n = new StringWrapper(
                        !string.IsNullOrEmpty(_sr.PreferredValue) ? _sr.PreferredValue : string.Empty
                        );

                    _values.Add(n);
                    EditableValues.Add(n);

                    ProcessAvailableCollection();
                },
                arg => true
                );

            DeleteCommand = new RelayCommand(
                arg =>
                {
                    var sw = arg as StringWrapper;

                    _values.Remove(sw);
                    EditableValues.Remove(sw);

                    ProcessAvailableCollection();
                },
                arg => true
                );
        }

        private void ProcessAvailableCollection()
        {
            AvailableValues.Clear();

            if (_values.Count < 2)
            {
                AvailableValues.Insert(0, new Option(_sr.Value, true, false));
            }

            if (!string.IsNullOrEmpty(_sr.PreferredValue))
            {
                AvailableValues.Add(new Option(_sr.PreferredValue, false, true));
            }

            AvailableValues.AddRange(_sr.PredefinedValues.Select(j => new Option(j, false, false)));
        }
    }
}