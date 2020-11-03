using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

namespace CacheService
{
    using System;

    public class Worker : BackgroundService
    {
        private const string PipeName = "CacheService";

        private readonly ILogger<Worker> _logger;

        private NamedPipeServerStream _pipe;
        private StreamWriter _streamWriter;
        private StreamReader _streamReader;

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

            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _pipe.Close();
            _isRunning = false;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Console.WriteLine("Start to wait connection...");
                await _pipe.WaitForConnectionAsync(stoppingToken);
                _streamWriter = new StreamWriter(_pipe);
                _streamReader = new StreamReader(_pipe);
                _streamWriter.AutoFlush = true;

                var buffer = new char[100];
                var numReadByte = await _streamReader.ReadAsync(buffer, stoppingToken);
                if (numReadByte > 0)
                {
                    _logger.LogInformation(buffer.ToString());
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            finally
            {
                _pipe.Close();
                _streamReader.Close();
                _streamWriter.Close();
            }
        }
    }
}
