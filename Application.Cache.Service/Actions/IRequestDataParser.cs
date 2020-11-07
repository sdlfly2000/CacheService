using Application.Cache.Service.Contracts;

namespace Application.Cache.Service.Actions
{
    public interface IRequestDataParser
    {
        RequestModel Parse(byte[] rev);
    }
}
