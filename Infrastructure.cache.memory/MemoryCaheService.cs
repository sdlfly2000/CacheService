using Common.Core.Cache;
using Common.Core.DependencyInjection;
using System.Collections.Generic;

namespace Infrastructure.cache.memory
{
    [ServiceLocate(typeof(IMemoryCaheService))]
    public class MemoryCaheService : IMemoryCaheService
    {
        private readonly ICacheService _cacheService;

        public MemoryCaheService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public string Get(string key)
        {
            var value = _cacheService.Get(key);
            return (string)value;
        }

        public IList<string> GetAllKeys()
        {
            return _cacheService.LoadAllKeys();
        }

        public string Remove(string key)
        {
            _cacheService.Remove(key);
            return string.Empty;
        }

        public string Set(string key, string value)
        {
            return _cacheService.Set(key, value);
        }
    }
}
