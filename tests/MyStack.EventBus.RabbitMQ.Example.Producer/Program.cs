using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.EventBus;
using Microsoft.Extensions.EventBus.RabbitMQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyStack.EventBus.RabbitMQ.Example;
using System.Reflection;

namespace Dragon.MessageTransport.RabbitMQ.Example.Producer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new HostBuilder()
               .ConfigureHostConfiguration(configure =>
               {
                   configure.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");
               })
               .ConfigureServices((context, services) =>
               {
                   services.AddLogging(logging =>
                   {
                       logging.AddConsole();
                   });
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
                   });
               });

            var app = builder.Build();

            var eventBus = app.Services.GetRequiredService<IEventBus>();
            eventBus.SendAsync(new HelloMessage()
            {
                Message = "Hello World"
            });
            // Publish a message and wait for a reply
            while (true)
            {
                var i = Console.ReadLine();
                if (i == "Q")
                    break;
                var pongMessage = eventBus.SendAsync(new Ping()
                {
                    SendBy = "A"
                }).GetAwaiter().GetResult();
                Console.WriteLine(pongMessage.ReplyBy);
            }
        }
    }
}