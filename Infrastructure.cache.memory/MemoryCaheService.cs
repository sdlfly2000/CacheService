using Common.Core.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Infrastructure.cache.memory
{
    [ServiceLocate(typeof(IMemoryCaheService))]
    public class MemoryCaheService : IMemoryCaheService
    {
        public MemoryCaheService()
        {

        }

        public T Get<T>(string key) where T : class
        {
            throw new NotImplementedException();
        }

        public IList<string> GetAllKeys()
        {
            throw new NotImplementedException();
        }

        public T Set<T>(string key, T value) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
