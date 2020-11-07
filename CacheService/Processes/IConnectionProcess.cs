using System;
using System.IO.Pipes;

namespace CacheService.Processes
{
    public interface IConnectionProcess
    {
        void SetNamePipeServerStream(NamedPipeServerStream pipe);

        void Process(IAsyncResult result);
    }
}
