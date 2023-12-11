using Microsoft.Extensions.EventBus;

namespace MyStack.EventBus.RabbitMQ.Example;

public class Ping : EventBase<Pong>
{
    public string SendBy { get; set; }
}