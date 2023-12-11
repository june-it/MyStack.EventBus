using System.Collections.Generic;

namespace Microsoft.Extensions.EventBus
{

    public abstract class EventBase : IEvent
    {
        public virtual Dictionary<string, object> Properties { get; set; }
        public EventBase()
        {
            Properties = new Dictionary<string, object>();
        }
    }
    public abstract class EventBase<TReply> : EventBase, IEvent<TReply>
           where TReply : class
    {
        public EventBase()
        {
            Properties = new Dictionary<string, object>();
        }
    }
}