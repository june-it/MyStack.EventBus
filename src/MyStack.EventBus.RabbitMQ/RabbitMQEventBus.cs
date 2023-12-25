using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.EventBus.RabbitMQ
{
    public class RabbitMQEventBus : IEventBus
    {
        private readonly string _exchangeName;
        private readonly string _routingKeyPrefix;
        private readonly IRoutingKeyProvider _routingKeyProvider;
        private readonly IRabbitMQProvider _rabbitMQProvider;
        private readonly ILogger _logger;
        public RabbitMQEventBus(IServiceProvider serviceProvider)
        {
            _rabbitMQProvider = serviceProvider.GetRequiredService<IRabbitMQProvider>();
            _routingKeyProvider = serviceProvider.GetService<IRoutingKeyProvider>();
            var options = serviceProvider.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
            _exchangeName = options.ExchangeOptions.Name;
            _routingKeyPrefix = options.RoutingKeyPrefix;
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            _logger = loggerFactory?.CreateLogger<RabbitMQEventBus>();
        }

        public async Task SendAsync(IEvent @event, CancellationToken cancellationToken = default)
        {
            using var channel = _rabbitMQProvider.CreateModel();
            cancellationToken.ThrowIfCancellationRequested();
            if (@event == null)
                throw new ArgumentNullException(nameof(@event), "The object of the event cannot be null");
            // Generate message routing key
            var routingKey = _routingKeyProvider.GetRoutingKey(@event.GetType());
            var sendData = JsonConvert.SerializeObject(@event);
            var sendBytes = Encoding.UTF8.GetBytes(sendData);
            var basicProperties = channel.CreateBasicProperties();
            basicProperties.Headers = new Dictionary<string, object>();
            if (@event.Properties != null)
            {
                foreach (var property in @event.Properties)
                {
                    basicProperties.Headers.TryAdd(property.Key, property.Value);
                }
            }
            channel.BasicPublish(_exchangeName, routingKey, false, null, sendBytes);
            _logger?.LogInformation($"[{routingKey}]Publish message: {sendData}。");
            await Task.CompletedTask;
        }

        public async Task<TReply> SendAsync<TReply>(IEvent<TReply> @event, CancellationToken cancellationToken = default) where TReply : class
        {
            using var channel = _rabbitMQProvider.CreateModel();
            cancellationToken.ThrowIfCancellationRequested();
            if (@event == null)
                throw new ArgumentNullException(nameof(@event), "The object of the event cannot be null");
            BlockingCollection<string> replyMessages = new BlockingCollection<string>();
            var replyQueueName = channel.QueueDeclare().QueueName;
            var routingKey = _routingKeyProvider.GetRoutingKey(@event.GetType());
            var properties = channel.CreateBasicProperties();

            properties.ReplyTo = Guid.NewGuid().ToString();
            properties.CorrelationId = Guid.NewGuid().ToString();
            properties.Headers = new Dictionary<string, object>();
            if (@event.Properties != null)
            {
                foreach (var property in @event.Properties)
                {
                    properties.Headers.TryAdd(property.Key, property.Value);
                }
            }

            channel.QueueBind(replyQueueName, _exchangeName, $"{_routingKeyPrefix}{properties.ReplyTo}");
            // Accept reply messages
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (_, ea) =>
            {
                var replyMessage = Encoding.UTF8.GetString(ea.Body.Span);
                if (ea.BasicProperties.CorrelationId == properties.CorrelationId)
                {
                    replyMessages.Add(replyMessage);
                }
                _logger?.LogInformation($"[{routingKey}]Received reply message: {replyMessage}。");
            };
            channel.BasicConsume(queue: replyQueueName, autoAck: true, consumer: consumer);


            // Sending a message
            var sendData = JsonConvert.SerializeObject(@event);
            var sendBytes = Encoding.UTF8.GetBytes(sendData);
            channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: sendBytes);
            _logger?.LogInformation($"[{routingKey}]Publish message: {sendData}。");

            var replyMessage = replyMessages.Take(cancellationToken);
            return await Task.FromResult(JsonConvert.DeserializeObject<TReply>(replyMessage));
        }
    }
}
