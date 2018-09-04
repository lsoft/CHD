using System.Collections.Concurrent;

namespace CHD.Common.KeyValueContainer
{
    public sealed class InMemoryKeyValueContainer : IKeyValueContainer
    {
        private readonly ConcurrentDictionary<string, string> _dict = new ConcurrentDictionary<string, string>();

        public InMemoryKeyValueContainer()
        {
        }

        public bool TryGet(string key, out string value)
        {
            return
                _dict.TryGetValue(key, out value);
        }

        public void AddOrUpdate(string key, string value)
        {
            _dict[key] = value;
        }
    }
}