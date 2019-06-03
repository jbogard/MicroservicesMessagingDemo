using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using Sales.Data;

namespace Sales
{
    class Program
    {
        static async Task Main()
        {
            Console.Title = "Sales";

            var services = new ServiceCollection().AddLogging().AddScoped<PlaceOrderHandler>();
            services.AddDbContext<OrdersDbContext>();

            var endpointConfiguration = new EndpointConfiguration("MicroservicesMessagingDemo.Sales");

            var transport = endpointConfiguration
                .UseTransport<RabbitMQTransport>()
                .ConnectionString("host=localhost");
            transport.UseConventionalRoutingTopology();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UsePersistence<InMemoryPersistence>();

            endpointConfiguration.UseContainer<ServicesBuilder>(
                customizations: customizations =>
                {
                    customizations.ExistingServices(services);
                });

            var endpointInstance = await Endpoint.Start(endpointConfiguration);

            var tcs = new TaskCompletionSource<object>();
            Console.CancelKeyPress += (sender, e) => { tcs.SetResult(null); };

            await Console.Out.WriteLineAsync("Press Ctrl+C to exit...");

            await tcs.Task;

            await endpointInstance.Stop();
        }
    }
}
