using System;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Cache.Service.Actions;
using Application.Cache.Service.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CacheService
{
    public class Worker : BackgroundService
    {
        private const string PipeName = "CacheService";
        private bool _isRunning = false;
        private NamedPipeServerStream _pipe;  
        
        private readonly ILogger<Worker> _logger;
        private readonly IRequestDataParser _requestDataParser;
        private readonly IRequestExecuteAction _requestExecuteAction;

        public Worker(
            ILogger<Worker> logger,
            IRequestDataParser requestDataParser,
            IRequestExecuteAction requstExecuteAction)
        {
            _logger = logger;
            _requestDataParser = requestDataParser;
            _requestExecuteAction = requstExecuteAction;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_isRunning && _pipe == null)
            {
                _pipe = new NamedPipeServerStream(PipeName, PipeDirection.InOut);
                _isRunning = true;
            }

            _ = base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_pipe != null)
            {                
                _pipe.Close();
                _pipe.Dispose();
                _isRunning = false;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Waiting for connection...");

            if (_isRunning && _pipe != null && !_pipe.IsConnected)
            {
                var _result = _pipe.BeginWaitForConnection(new AsyncCallback(ProcessConnection), null);                
            }
        }

        private void ProcessConnection(IAsyncResult result)
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

            var asyncResult = _pipe.BeginWaitForConnection(new AsyncCallback(ProcessConnection), null);
        }
    }
}
