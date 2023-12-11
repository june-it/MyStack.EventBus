using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyStack.EventBus.RabbitMQ;
using System;
using System.Reflection;

namespace Microsoft.Extensions.EventBus.RabbitMQ
{
    public class DefaultRoutingKeyProvider : IRoutingKeyProvider
    {
        private readonly string _routingKeyPrefix;
        public DefaultRoutingKeyProvider(IServiceProvider serviceProvider)
        {
            var options = serviceProvider.GetRequiredService<IOptions<RabbitMQOptions>>();
            _routingKeyPrefix = options.Value.RoutingKeyPrifix;
        }
        public string GetRoutingKey(Type eventType)
        {
            var eventNameAttribute = eventType.GetCustomAttribute<EventNameAttribute>();
            if (eventNameAttribute != null)
            {
                return $"{_routingKeyPrefix}{eventNameAttribute.Name}";
            }
            return $"{_routingKeyPrefix}{eventType.FullName}";
        }
    }
}
