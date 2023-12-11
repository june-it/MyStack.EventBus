using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{

    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Add MyStack.EventBus
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection</param>
        /// <param name="configure">The Microsoft.Extensions.EventBus.IEventBuilder configuration delegate</param>
        /// <param name="assemblies">Assemblies to scan</param>
        /// <returns>The Microsoft.Extensions.DependencyInjection.IServiceCollection</returns>
        public static IServiceCollection AddEventBus(this IServiceCollection services, Action<IEventBusConfigurator> configure, params Assembly[] assemblies)
        {
            var configurator = new EventBusConfigurator(services);

            List<SubscriptionInfo> subscriptions = new List<SubscriptionInfo>();
            var eventHandlerTypes = assemblies.SelectMany(x => x.GetTypes().Where(x => x.GetInterfaces().Any(y => y.IsGenericType && (y.GetGenericTypeDefinition() == typeof(IEventHandler<>) || y.GetGenericTypeDefinition() == typeof(IEventHandler<,>)))));
            if (eventHandlerTypes.Any())
            {
                foreach (var eventHandlerType in eventHandlerTypes)
                {
                    var handlerInterfaces = eventHandlerType.GetInterfaces()
                        .Where(x => x.IsGenericType && (x.GetGenericTypeDefinition() == typeof(IEventHandler<>) || x.GetGenericTypeDefinition() == typeof(IEventHandler<,>)));
                    foreach (var handlerInterface in handlerInterfaces)
                    {
                        if (handlerInterface.GetGenericArguments().Length == 1)
                        {
                            services.AddTransient(typeof(IEventHandler<>).MakeGenericType(handlerInterface.GetGenericArguments()), eventHandlerType);
                            subscriptions.Add(new SubscriptionInfo(handlerInterface.GetGenericArguments()[0], typeof(IEventHandler<>)));
                        }
                        else if (handlerInterface.GetGenericArguments().Length == 2)
                        {
                            services.AddTransient(typeof(IEventHandler<,>).MakeGenericType(handlerInterface.GetGenericArguments()), eventHandlerType);
                            subscriptions.Add(new SubscriptionInfo(handlerInterface.GetGenericArguments()[0], typeof(IEventHandler<,>), handlerInterface.GetGenericArguments()[1]));
                        }
                    }
                }
            }
            services.AddSingleton<ISubscriptionRegistrar, SubscriptionManager>();
            services.AddSingleton<ISubscriptionManager>(factory =>
            {
                var subscriptionRegistrar = factory.GetRequiredService<ISubscriptionRegistrar>();
                subscriptionRegistrar.Register(subscriptions);
                return (SubscriptionManager)subscriptionRegistrar;
            });
            configure?.Invoke(configurator);
            services.TryAddSingleton<IConsumerServer, DefaultConsumerServer>();
            services.TryAddTransient<IEventBus, DefaultEventBus>();

            // Start message listening service
            if (eventHandlerTypes.Any())
            {
                var serviceProvider = configurator.Services.BuildServiceProvider();
                var consumerServer = serviceProvider.GetRequiredService<IConsumerServer>();
                consumerServer.StartAsync().Wait();
            }
            return services;
        }

    }
}

