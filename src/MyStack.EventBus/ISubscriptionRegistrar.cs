using System.Collections.Generic;

namespace Microsoft.Extensions.EventBus
{
    public interface ISubscriptionRegistrar
    {
        void Register(List<SubscriptionInfo> subscriptions);
    }
}
