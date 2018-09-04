namespace CHD.Installer.ViewModel.Edit
{
    public sealed class Option
    {
        public string Value
        {
            get;
            private set;
        }

        public bool IsInUse
        {
            get;
            private set;
        }

        public bool IsPreferred
        {
            get;
            private set;
        }

        public string FullValue
        {
            get
            {
                return
                    string.Format(
                        "{0}{1}{2}",
                        string.IsNullOrEmpty(Value) ? "<�����>" : Value,
                        IsInUse ? " [���������� ��������]" : string.Empty,
                        IsPreferred ? " [���������������� ��������]" : string.Empty
                        );
            }
        }

        public Option(string value, bool isInUse, bool isPreferred)
        {
            Value = value;
            IsInUse = isInUse;
            IsPreferred = isPreferred;
        }

        public override string ToString()
        {
            return
                Value;
        }
    }
}