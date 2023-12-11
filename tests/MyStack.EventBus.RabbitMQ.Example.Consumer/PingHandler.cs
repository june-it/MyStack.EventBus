using Microsoft.Extensions.EventBus;
using MyStack.EventBus.RabbitMQ.Example;

namespace Dragon.MessageTransport.RabbitMQ.Example.Consumer;

public class PingHandler : IEventHandler<Ping, Pong>
{

    public Task<Pong> HandleAsync(Ping message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Ping");
        return Task.FromResult(new Pong() { ReplyBy = "B" });
    }
}
