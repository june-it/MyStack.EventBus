using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Microsoft.Extensions.EventBus.RabbitMQ
{

    public class DefaultRabbitMQProvider : IRabbitMQProvider
    {
        private readonly IConnection _connection;
        public DefaultRabbitMQProvider(IOptions<RabbitMQOptions> options)
        {
            var factory = new ConnectionFactory
            {
                UserName = options.Value.UserName,
                Password = options.Value.Password,
                HostName = options.Value.HostName,
                Port = options.Value.Port,
                AutomaticRecoveryEnabled = true
            };
            if (!string.IsNullOrEmpty(options.Value.VirtualHost))
                factory.VirtualHost = options.Value.VirtualHost;
            _connection = factory.CreateConnection();
        }
        public IModel CreateModel()
        {
            return _connection.CreateModel();
        }
        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }

    }
}

