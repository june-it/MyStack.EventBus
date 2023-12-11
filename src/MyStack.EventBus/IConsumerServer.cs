using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.EventBus
{
    public interface IConsumerServer
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
