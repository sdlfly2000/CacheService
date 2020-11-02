using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CacheService
{
    public class Worker : BackgroundService
    {
        private const string PipeName = "CacheService";

        private readonly ILogger<Worker> _logger;
        private NamedPipeServerStream _pipe;

        private bool _isRunning = false;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_isRunning && _pipe == null)
            {
                _pipe = new NamedPipeServerStream(PipeName, PipeDirection.InOut);
                _isRunning = true;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_isRunning && _pipe != null)
            {
                _pipe.Flush();
                _pipe.Close();
                _isRunning = false;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if(_isRunning && _pipe != null)
            {
                if (!_pipe.IsConnected)
                {
                    _pipe.Flush();
                    await _pipe.WaitForConnectionAsync();
                    _pipe.BeginWaitForConnection
                }
            }
        }
    }
}
