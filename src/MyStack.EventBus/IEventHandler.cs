using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.EventBus
{
    public interface IEventHandler
    {

    }
    /// <summary>
    /// Represents an event handler interface
    /// </summary>
    /// <typeparam name="TEvent">The type of event</typeparam>
    public interface IEventHandler<TEvent>: IEventHandler
        where TEvent : class, IEvent
    {
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
    }
    /// <summary>
    /// Represents an event handler interface
    /// </summary>
    /// <typeparam name="TEvent">The type of event</typeparam>
    /// <typeparam name="TReply">The type of reply</typeparam>
    public interface IEventHandler<TEvent, TReply>: IEventHandler
        where TEvent : class, IEvent<TReply>
        where TReply : class
    {
        Task<TReply> HandleAsync(TEvent message, CancellationToken cancellationToken = default);
    }
}