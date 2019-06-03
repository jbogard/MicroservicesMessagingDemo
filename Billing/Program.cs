using System;
using System.Threading.Tasks;
using NServiceBus;

namespace Billing
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Billing";

            var endpointConfiguration = new EndpointConfiguration("MicroservicesMessagingDemo.Billing");

            var transport = endpointConfiguration
                .UseTransport<RabbitMQTransport>()
                .ConnectionString("host=localhost");
            transport.UseConventionalRoutingTopology();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UsePersistence<InMemoryPersistence>();

            var endpointInstance = await Endpoint.Start(endpointConfiguration);

            var tcs = new TaskCompletionSource<object>();
            Console.CancelKeyPress += (sender, e) => { tcs.SetResult(null); };

            await Console.Out.WriteLineAsync("Press Ctrl+C to exit...");

            await tcs.Task;
            await endpointInstance.Stop();
        }
    }
}
 