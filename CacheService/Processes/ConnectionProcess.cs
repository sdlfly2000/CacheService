using Application.Cache.Service;
using Common.Core.Cache.Client.Contracts;
using Common.Core.DependencyInjection;
using System;
using System.IO.Pipes;
using System.Text;

namespace CacheService.Processes
{
    [ServiceLocate(typeof(IConnectionProcess))]
    public class ConnectionProcess : IConnectionProcess
    {
        private NamedPipeServerStream _pipe;
        private readonly ISharedCacheService _service;

        public ConnectionProcess(ISharedCacheService service)
        {
            _service = service;
        }

        public void SetNamePipeServerStream(NamedPipeServerStream pipe)
        {
            _pipe = pipe;
        }

        public void Process(IAsyncResult result)
        {
            _pipe.WaitForPipeDrain();

            var lenBuffer = new byte[2];
            _pipe.Read(lenBuffer, 0, 2);

            var lenBufferInInt = BitConverter.ToInt16(lenBuffer) - 2;
            var buffer = new byte[lenBufferInInt];
            _pipe.Read(buffer, 0, lenBufferInInt);

            var parser = _service.Parse(buffer);

            switch (parser.CommandCode)
            {
                case CommandType.Get:
                    var response = _service.Get(parser.Key);
                    _pipe.Write(Encoding.UTF8.GetBytes(response));
                    break;

                case CommandType.Set:
                    _service.Set(parser.Key, parser.Value);
                    break;

                case CommandType.GetAllKeys:
                    var allKeys = _service.GetAllKeys();
                    _pipe.Write(
                        Encoding.UTF8.GetBytes(
                            System.Text.Json.JsonSerializer.Serialize(allKeys)));
                    break;

                case CommandType.Remove:
                    _service.Remove(parser.Key);
                    break;

            }

            _pipe.Disconnect();

            _pipe.EndWaitForConnection(result);

            var asyncResult = _pipe.BeginWaitForConnection(new AsyncCallback(Process), null);
        }
    }
}
