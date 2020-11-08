using Application.Cache.Service.Contracts;
using System.Collections.Generic;

namespace Application.Cache.Service
{
    public interface ISharedCacheService
    {
        RequestModel Parse(byte[] value);

        string Get(string key);

        string Set(string key, string value);

        string Remove(string key);

        IList<string> GetAllKeys();
    }
}
