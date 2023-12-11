using Microsoft.Extensions.DependencyInjection;
using MyStack.EventBus.RabbitMQ;
using System;

namespace Microsoft.Extensions.EventBus.RabbitMQ
{
    public static class IEventBusConfiguratorExtensions
    {
        public static void AddRabbtMQ(this IEventBusConfigurator  configurator, Action<RabbitMQOptions> configure)
        {
            var options = new RabbitMQOptions();
            configure.Invoke(options);
            configurator.Services.Configure(configure);
            configurator.Services.Add(new ServiceDescriptor(typeof(IRabbitMQProvider), typeof(DefaultRabbitMQProvider), ServiceLifetime.Transient));
            configurator.Services.Add(new ServiceDescriptor(typeof(IEventBus), typeof(RabbitMQEventBus), ServiceLifetime.Transient));
            configurator.Services.Add(new ServiceDescriptor(typeof(IRoutingKeyProvider), typeof(DefaultRoutingKeyProvider), ServiceLifetime.Transient));
            configurator.Services.Add(new ServiceDescriptor(typeof(IConsumerServer), typeof(RabbitMQConsumerServer), ServiceLifetime.Singleton));

        }
    }
}
