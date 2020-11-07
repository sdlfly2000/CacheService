using Application.Cache.Service.Actions;
using Application.Cache.Service.Contracts;
using Common.Core.DependencyInjection;
using System;

namespace Application.Cache.Service
{
    [ServiceLocate(typeof(ISharedCacheService))]
    public class SharedCacheService : ISharedCacheService
    {
        private RequestModel _requestModel;
        private readonly IRequestDataParser _revParser;

        public byte[] Object { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SharedCacheService(IRequestDataParser revParser)
        {
            _revParser = revParser;
        }

        public CommandType Parse(byte[] revData)
        {
            _requestModel = _revParser.Parse(revData);
            return _requestModel.CommandCode;
        }
    }
}
