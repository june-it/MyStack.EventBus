using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.EventBus
{
    public class EventBusConfigurator : IEventBusConfigurator
    {
        public IServiceCollection Services { get; }
        public EventBusConfigurator(IServiceCollection services)
        {
            Services = services;
        }
    }
}
