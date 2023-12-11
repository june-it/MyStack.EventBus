using System;

namespace Microsoft.Extensions.EventBus.RabbitMQ
{
    public interface IRoutingKeyProvider
    {
        string GetRoutingKey(Type eventType);
    }

}
