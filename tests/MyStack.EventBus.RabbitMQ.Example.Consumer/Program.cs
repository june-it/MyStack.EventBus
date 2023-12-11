using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.EventBus;
using Microsoft.Extensions.EventBus.RabbitMQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyStack.EventBus.RabbitMQ.Example.Consumer;
using System.Reflection;

namespace Dragon.MessageTransport.RabbitMQ.Example.Consumer
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
                   }, 
                   Assembly.GetExecutingAssembly()
                   );
               });

            var app = builder.Build();
            app.Run();
        }
    }
}