using System;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Cache.Service.Actions;
using Application.Cache.Service.Contracts;
using CacheService.Processes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CacheService
{
    public class Worker : BackgroundService
    {
        private const string PipeName = "CacheService";
        private bool _isRunning = false;

        private NamedPipeServerStream _pipe;

        private readonly IConnectionProcess _process;        
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger, IConnectionProcess process)
        {
            _logger = logger;
            _process = process;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_isRunning && _pipe == null)
            {
                _pipe = new NamedPipeServerStream(PipeName, PipeDirection.InOut);
                _isRunning = true;

                _process.SetNamePipeServerStream(_pipe);
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
                var _result = _pipe.BeginWaitForConnection(new AsyncCallback(_process.Process), null);                
            }
        }
    }
}
