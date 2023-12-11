using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.EventBus
{
    public interface IEventBusConfigurator
    {
        IServiceCollection Services { get; } 
    }
}
