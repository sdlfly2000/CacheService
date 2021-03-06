﻿using System.Collections.Generic;

namespace Infrastructure.cache.memory
{
    public interface IMemoryCaheService
    {
        string Get(string key);

        string Set(string key, string value);

        string Remove(string key);

        IList<string> GetAllKeys();
    }
}
