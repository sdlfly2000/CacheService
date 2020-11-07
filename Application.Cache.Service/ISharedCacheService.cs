using Application.Cache.Service.Contracts;

namespace Application.Cache.Service
{
    public interface ISharedCacheService
    {
        byte[] Object { get; set; }

        CommandType Parse(byte[] revData);
    }
}
