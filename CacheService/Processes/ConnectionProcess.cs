using Application.Cache.Service.Actions;
using Application.Cache.Service.Contracts;
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
        private readonly IRequestDataParser _requestDataParser;
        private readonly IRequestExecuteAction _requestExecuteAction;

        public ConnectionProcess(
            IRequestDataParser requestDataParser,
            IRequestExecuteAction requstExecuteAction)
        {
            _requestDataParser = requestDataParser;
            _requestExecuteAction = requstExecuteAction;
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

            var parser = _requestDataParser.Parse(buffer);

            switch (parser.CommandCode)
            {
                case CommandType.Get:
                    var response = _requestExecuteAction.Get(parser.Key);
                    _pipe.Write(Encoding.UTF8.GetBytes(response));
                    break;

                case CommandType.Set:
                    _requestExecuteAction.Set(parser.Key, parser.Value);
                    break;

                case CommandType.GetAllKeys:
                    var allKeys = _requestExecuteAction.GetAllKeys();
                    _pipe.Write(
                        Encoding.UTF8.GetBytes(
                            System.Text.Json.JsonSerializer.Serialize(allKeys)));
                    break;

                case CommandType.Remove:
                    _requestExecuteAction.Remove(parser.Key);
                    break;

            }

            _pipe.Disconnect();

            _pipe.EndWaitForConnection(result);

            var asyncResult = _pipe.BeginWaitForConnection(new AsyncCallback(Process), null);
        }
    }
}
