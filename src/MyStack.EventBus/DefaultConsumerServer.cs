using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.EventBus
{
    public abstract class DefaultConsumerServer : IConsumerServer
    {

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
