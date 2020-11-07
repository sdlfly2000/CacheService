using Common.Core.DependencyInjection;
using Infrastructure.cache.memory;
using System.Collections.Generic;

namespace Application.Cache.Service.Actions
{
    [ServiceLocate(typeof(IRequestExecuteAction))]
    public class RequestExecuteAction : IRequestExecuteAction
    {
        private readonly IMemoryCaheService _memoryCaheService;

        public RequestExecuteAction(IMemoryCaheService memoryCacheService)
        {
            _memoryCaheService = memoryCacheService;
        }

        public string Get(string key)
        {
            return _memoryCaheService.Get(key);
        }

        public IList<string> GetAllKeys()
        {
            return _memoryCaheService.GetAllKeys();
        }

        public string Remove(string key)
        {
            return _memoryCaheService.Remove(key);
        }

        public string Set(string key, string value)
        {
            return _memoryCaheService.Set(key, value);
        }
    }
}
