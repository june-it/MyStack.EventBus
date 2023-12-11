using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.EventBus;
using Microsoft.Extensions.EventBus.RabbitMQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyStack.EventBus.RabbitMQ
{
    public class RabbitMQConsumerServer : IConsumerServer
    {
        private IModel _channel;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRabbitMQProvider _rabbitMQProvider;
        private readonly IRoutingKeyProvider _routingKeyProvider;
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly ILogger _logger;
        private static Dictionary<string, Type> _cache = new Dictionary<string, Type>();
        public RabbitMQConsumerServer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _rabbitMQProvider = _serviceProvider.GetRequiredService<IRabbitMQProvider>();
            _routingKeyProvider = _serviceProvider.GetRequiredService<IRoutingKeyProvider>();
            _subscriptionManager = _serviceProvider.GetRequiredService<ISubscriptionManager>();
            Options = _serviceProvider.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            _logger = loggerFactory?.CreateLogger(GetType().Name);

        }
        public RabbitMQOptions Options { get; private set; }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var allSubscriptions = _subscriptionManager.GetAllSubscriptions();
            if (!allSubscriptions.Any())
                return Task.CompletedTask;

            _channel = _rabbitMQProvider.CreateModel();

            _channel.ExchangeDeclare(Options.ExchangeOptions.Name, Options.ExchangeOptions.ExchangeType, Options.ExchangeOptions.Durable, Options.ExchangeOptions.AutoDelete, Options.ExchangeOptions.Arguments);

            _channel.QueueDeclare(Options.QueueOptions.Name, Options.QueueOptions.Durable, Options.QueueOptions.Exclusive, Options.QueueOptions.AutoDelete, Options.QueueOptions.Arguments);

            foreach (var subscription in allSubscriptions)
            {
                var routingKey = _routingKeyProvider.GetRoutingKey(subscription.EventType);
                _channel.QueueBind(Options.QueueOptions.Name, Options.ExchangeOptions.Name, routingKey);
                _cache.Add(routingKey, subscription.EventType);
                _logger?.LogInformation($"Bind routing key named {routingKey} to queue named {Options.QueueOptions.Name} ");
            }
           

            EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
            _channel.BasicConsume(queue: Options.QueueOptions.Name, autoAck: false, consumer);
            consumer.Received += async (_, e) =>
            {
                var receivedMessage = Encoding.UTF8.GetString(e.Body.Span);
                _logger?.LogDebug($"Received message: {receivedMessage}。");
                var subscriptions = GetSubscriptionInfos(e.RoutingKey);
                if (subscriptions != null)
                {
                    foreach (var subscription in subscriptions)
                    {
                        var @event = JsonConvert.DeserializeObject(receivedMessage, subscription.EventType);
                        if (subscription.ReplyType == null)
                        {
                            var eventHandlerType = subscription.EventHandlerType.MakeGenericType(subscription.EventType);
                            var eventHandler = _serviceProvider.GetRequiredService(eventHandlerType);

                            await ((dynamic)eventHandler).HandleAsync((dynamic)@event, cancellationToken);
                            _channel.BasicAck(e.DeliveryTag, false);
                        }
                        else
                        {
                            string replyMessage = "";
                            try
                            {
                                var eventHandlerType = subscription.EventHandlerType.MakeGenericType(subscription.EventType, subscription.ReplyType);
                                var eventHandler = _serviceProvider.GetRequiredService(eventHandlerType);
                                var replyMessageObj = await ((dynamic)eventHandler).HandleAsync((dynamic)@event, cancellationToken);
                                replyMessage = JsonConvert.SerializeObject(replyMessageObj);
                            }
                            finally
                            {
                                var properties = e.BasicProperties;
                                var replyProperties = _channel.CreateBasicProperties();
                                replyProperties.CorrelationId = properties.CorrelationId;
                                _logger?.LogDebug($"Reply message: {replyMessage}。");
                                var replyBytes = Encoding.UTF8.GetBytes(replyMessage);
                                _channel.BasicPublish(exchange: Options.ExchangeOptions.Name, routingKey: properties.ReplyTo, mandatory: false, basicProperties: replyProperties, body: replyBytes);
                                _channel.BasicAck(e.DeliveryTag, false);
                            }
                        }
                    }
                }
            };
            return Task.CompletedTask;
        } 

        private IList<SubscriptionInfo> GetSubscriptionInfos(string routingKey)
        {
            if (_cache.TryGetValue(routingKey, out var eventType))
            {
                return _subscriptionManager.GetSubscriptions(eventType);
            }
            return null;
        }
        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            _channel?.Dispose();
            return Task.CompletedTask;
        }
    }

}