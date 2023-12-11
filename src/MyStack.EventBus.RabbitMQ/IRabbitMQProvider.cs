using RabbitMQ.Client;
using System;

namespace Microsoft.Extensions.EventBus.RabbitMQ
{
    public interface IRabbitMQProvider : IDisposable
    {
        IModel CreateModel();
    }
}


