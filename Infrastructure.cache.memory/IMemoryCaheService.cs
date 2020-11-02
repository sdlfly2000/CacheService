using System.Collections.Generic;

namespace Infrastructure.cache.memory
{
    public interface IMemoryCaheService
    {
        T Get<T>(string key) where T : class;

        T Set<T>(string key, T value) where T : class;

        IList<string> GetAllKeys();
    }
}
