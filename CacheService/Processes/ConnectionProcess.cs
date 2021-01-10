using Application.Cache.Service;
using Application.Cache.Service.Contracts;
using Common.Core.Cache.Client.Contracts;
using Common.Core.Cache.Client.Utils;
using Common.Core.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<Worker> _logger;

        public ConnectionProcess(
            ISharedCacheService service,
            ILogger<Worker> logger)
        {
            _service = service;
            _logger = logger;
        }

        public void SetNamePipeServerStream(NamedPipeServerStream pipe)
        {
            _pipe = pipe;
        }

        public void Process(IAsyncResult result)
        {
            _logger.LogInformation("Connected...");

            try
            {
                var lenBuffer = new byte[2];
                _pipe.Read(lenBuffer, 0, 2);

                var lenBufferInInt = BitConverter.ToInt16(lenBuffer) - 2;
                var buffer = new byte[lenBufferInInt];
                _pipe.Read(buffer, 0, lenBufferInInt);

                var parser = _service.Parse(buffer);

                _logger.LogInformation(GetFormattedRevData(parser));

                switch (parser.CommandCode)
                {
                    case CommandType.Get:
                        var response = _service.Get(parser.Key) ?? string.Empty;                         
                        _pipe.Write(ConvertTools.StringToBytes(response));                        
                        _logger.LogInformation("Response - | " + response);
                        break;

                    case CommandType.Set:
                        _service.Set(parser.Key, parser.Value);
                        break;

                    case CommandType.GetAllKeys:
                        var allKeys = _service.GetAllKeys();
                        var allKeysSerialized = System.Text.Json.JsonSerializer.Serialize(allKeys);
                        _pipe.Write(ConvertTools.StringToBytes(allKeysSerialized));
                        _logger.LogInformation("Response - | " + allKeysSerialized);
                        break;

                    case CommandType.Remove:
                        _service.Remove(parser.Key);
                        break;

                }

                _pipe.Disconnect();

                _logger.LogInformation("Disconnect.");

                _pipe.EndWaitForConnection(result);

            }catch(Exception e)
            {
                _logger.LogInformation(e.Message);
            }
            finally
            {
                var asyncResult = _pipe.BeginWaitForConnection(new AsyncCallback(Process), null);
            }            
        }

        #region Private Methods

        private string GetFormattedRevData(RequestModel model)
        {
            var formatted = Environment.NewLine;
            formatted += @"| Request - " + Environment.NewLine;
            formatted += @"| Command: " + model.CommandCode + Environment.NewLine;
            formatted += @"| Key: " + model.Key + Environment.NewLine;
            formatted += @"| Value: " + model.Value + Environment.NewLine;

            return formatted;
        }

        #endregion
    }
}
