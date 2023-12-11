using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.EventBus
{
    public class DefaultEventBus : IEventBus
    {
        public async Task SendAsync(IEvent @event, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
        }

        public async Task<TReply> SendAsync<TReply>(IEvent<TReply> @event, CancellationToken cancellationToken = default) where TReply : class
        {
            return await Task.FromResult<TReply>(default);
        }
    }
}
