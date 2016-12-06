namespace CHD.Common.KeyValueContainer
{
    public interface IKeyValueContainer
    {
        bool TryGet(
            string key, 
            out string value
            );

        void Add(
            string key,
            string value
            );
    }
}