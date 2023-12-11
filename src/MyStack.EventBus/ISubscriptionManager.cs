using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.EventBus
{
    public interface ISubscriptionManager
    {
        IList<SubscriptionInfo> GetSubscriptions(Type eventType);
        IList<SubscriptionInfo> GetAllSubscriptions();
    } 

}
