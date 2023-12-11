using System.Collections.Generic;

namespace MyStack.EventBus.RabbitMQ
{

    public class RabbitMQOptions
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; }
        public string VirtualHost { get; set; }
        public string RoutingKeyPrifix { get; set; }
        public RabbitMQExchangeOptions ExchangeOptions { get; set; } = new RabbitMQExchangeOptions();
        public RabbitMQQueueOptions QueueOptions { get; set; } = new RabbitMQQueueOptions();
    }

    public class RabbitMQExchangeOptions
    {
        public string ExchangeType { get; set; }
        public string Name { get; set; }
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
        public Dictionary<string, object> Arguments { get; set; }
    }
    public class RabbitMQQueueOptions
    {
        public string Name { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public Dictionary<string, object> Arguments { get; set; }
    }
}
