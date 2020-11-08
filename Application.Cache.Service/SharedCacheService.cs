using Application.Cache.Service.Actions;
using Application.Cache.Service.Contracts;
using Common.Core.DependencyInjection;
using System.Collections.Generic;

namespace Application.Cache.Service
{
    [ServiceLocate(typeof(ISharedCacheService))]
    public class SharedCacheService : ISharedCacheService
    {
        private readonly IRequestDataParser _requestDataParser;
        private readonly IRequestExecuteAction _requestExecuteAction;

        public SharedCacheService(
            IRequestDataParser requestDataParser,
            IRequestExecuteAction requestExecuteAction)
        {
            _requestDataParser = requestDataParser;
            _requestExecuteAction = requestExecuteAction;
        }

        public RequestModel Parse(byte[] value)
        {
            return _requestDataParser.Parse(value);
        }

        public string Get(string key)
        {
            return _requestExecuteAction.Get(key);
        }

        public string Set(string key, string value)
        {
            return _requestExecuteAction.Set(key, value);
        }

        public string Remove(string key)
        {
            return _requestExecuteAction.Remove(key);
        }

        public IList<string> GetAllKeys()
        {
            return _requestExecuteAction.GetAllKeys();
        }
    }
}
