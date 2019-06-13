using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Persistence.Sql;

namespace Shipping
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Shipping";

            var endpointConfiguration = new EndpointConfiguration("MicroservicesMessagingDemo.Shipping");

            var transport = endpointConfiguration
                .UseTransport<RabbitMQTransport>()
                .ConnectionString("host=localhost");
            transport.UseConventionalRoutingTopology();
            endpointConfiguration.EnableInstallers();
            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            persistence.ConnectionBuilder(() => new SqlConnection(@"Data Source=(localdb)\mssqllocaldb;Initial Catalog=ShippingDb;Integrated Security=True"));

            var endpointInstance = await Endpoint.Start(endpointConfiguration);

            var tcs = new TaskCompletionSource<object>();
            Console.CancelKeyPress += (sender, e) => { tcs.SetResult(null); };

            await Console.Out.WriteLineAsync("Press Ctrl+C to exit...");

            await tcs.Task;
            await endpointInstance.Stop();
        }
    }
}
