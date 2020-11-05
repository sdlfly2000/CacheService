using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.cache.memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CacheService
{
    public class Worker : BackgroundService
    {
        private const string PipeName = "CacheService";
        private bool _isRunning = false;
        private byte[] _buffer = new byte[20];

        private NamedPipeServerStream _pipe;        
        private readonly ILogger<Worker> _logger;
        private readonly IMemoryCaheService _memoryService;

        public Worker(
            ILogger<Worker> logger,
            IMemoryCaheService memoryService)
        {
            _logger = logger;
            _memoryService = memoryService;
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
            var numRead = _pipe.Read(_buffer, 0, 20);
            if (numRead > 0)
            {
                Console.WriteLine(System.Text.Encoding.Default.GetString(_buffer));
            }

            _pipe.Disconnect();

            _pipe.EndWaitForConnection(result);

            var asyncResult = _pipe.BeginWaitForConnection(new AsyncCallback(ProcessConnection), null);
        }
    }
}
