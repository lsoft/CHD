namespace CHD.FileSystem.Watcher
{
    public interface IExcluder
    {
        void AddToExcluded(
            string filepath
            );

        void RemoveFromExcluded(
            string filepath
            );
    }
}