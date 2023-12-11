using Microsoft.Extensions.EventBus;

namespace MyStack.EventBus.RabbitMQ.Example;

[EventName("HelloMessage")]
public class HelloMessage : EventBase
{
    public string Message { get; set; }
}
