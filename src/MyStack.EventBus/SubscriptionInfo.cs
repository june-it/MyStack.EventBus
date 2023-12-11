using System;

namespace Microsoft.Extensions.EventBus
{
    public class SubscriptionInfo
    {
        public SubscriptionInfo(Type eventType, Type eventHandlerType)
        {
            EventType = eventType;
            EventHandlerType = eventHandlerType;
        }
        public SubscriptionInfo(Type eventType, Type eventHandlerType, Type replyType)
        {
            EventType = eventType;
            EventHandlerType = eventHandlerType;
            ReplyType = replyType;
        }

        public Type EventType { get; set; }
        public Type EventHandlerType { get; set; }
        public Type ReplyType { get; set; }
    }
}
