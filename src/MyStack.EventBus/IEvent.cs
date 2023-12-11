using System.Collections.Generic;

namespace Microsoft.Extensions.EventBus
{
    /// <summary>
    /// Represents an event interface
    /// </summary>
    public interface IEvent
    {
        Dictionary<string, object> Properties { get; }
    }
    /// <summary>
    /// Represents a responsive event interface
    /// </summary>
    /// <typeparam name="TReply"></typeparam>
    public interface IEvent<TReply> : IEvent
         where TReply : class
    {
    }
}

