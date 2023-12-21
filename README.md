# MyStack.EventBus

This package provides the APIs for using event bus library


| nuget      | stats |
| ----------- | ----------- |
| [![nuget](https://img.shields.io/nuget/v/MyStack.EventBus.svg?style=flat-square)](https://www.nuget.org/packages/MyStack.EventBus)        | [![stats](https://img.shields.io/nuget/dt/MyStack.EventBus.svg?style=flat-square)](https://www.nuget.org/stats/packages/MyStack.EventBus?groupby=Version)        |
| [![nuget](https://img.shields.io/nuget/v/MyStack.EventBus.RabbitMQ.svg?style=flat-square)](https://www.nuget.org/packages/MyStack.EventBus.RabbitMQ)    | [![stats](https://img.shields.io/nuget/dt/MyStack.EventBus.RabbitMQ.svg?style=flat-square)](https://www.nuget.org/stats/packages/DMyStack.EventBus.RabbitMQ?groupby=Version)         |

# Usage

## Add Services to container
``` 
services.AddEventBus(builder =>
 {
     builder.AddRabbtMQ(configure =>
     {
         configure.HostName = "localhost";
         configure.VirtualHost = "/";
         configure.Port = 5672;
         configure.UserName = "admin";
         configure.Password = "admin";
         configure.QueueOptions.Name = "MyStack";
         configure.ExchangeOptions.Name = "MyStack";
         configure.ExchangeOptions.ExchangeType = "topic";
     });
 },Assembly.GetExecutingAssembly());
```
## Define a message
```
[EventName("HelloMessage")]
public class HelloMessage : EventBase
{
    public string Message { get; set; }
}

```
## Subscribe to a message
```
  public class HelloMessageHandler : IEventHandler<HelloMessage>
    {
        public async Task HandleAsync(HelloMessage message, CancellationToken cancellationToken)
        {
            Console.WriteLine("Hello");
            await Task.CompletedTask;
        }
    }
```
## Publish a message
```
var bus = serviceProvider.GetRequiredService<IEventBus>();
await bus.PublishAsync(new HelloMessage() { Message = "Hello" });
```
# License

MIT