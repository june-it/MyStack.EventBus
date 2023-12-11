using Microsoft.Extensions.EventBus;

namespace MyStack.EventBus.RabbitMQ.Example.Consumer
{
    public class HelloMessageHandler : IEventHandler<HelloMessage>
    {
        public async Task HandleAsync(HelloMessage message, CancellationToken cancellationToken)
        {
            Console.WriteLine("Hello");
            await Task.CompletedTask;
        }
    }
}
