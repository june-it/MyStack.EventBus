using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.EventBus
{
    public interface IEventBus
    {
        Task SendAsync(IEvent @event, CancellationToken cancellationToken = default);
        Task<TReply> SendAsync<TReply>(IEvent<TReply> @event, CancellationToken cancellationToken = default)
            where TReply : class;
    }
}

