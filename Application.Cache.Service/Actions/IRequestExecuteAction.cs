using System.Collections.Generic;

namespace Application.Cache.Service.Actions
{
    public interface IRequestExecuteAction
    {
        string Get(string key);

        string Set(string key, string value);

        string Remove(string key);

        IList<string> GetAllKeys();
    }
}
