namespace CHD.Common.Diff.Conflict
{
    public interface IConflictDescription
    {
        bool ConflictExists
        {
            get;
        }

        void RaiseExceptionIfConfictExists();
    }
}