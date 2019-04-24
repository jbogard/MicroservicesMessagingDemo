using System.Threading.Tasks;
using Billing.Events;
using NServiceBus;
using NServiceBus.Logging;
using Sales.Events;

namespace Billing
{
    public class OrderCompletedHandler : IHandleMessages<OrderCompleted>
    {
        private static readonly ILog Log = LogManager.GetLogger<OrderCompletedHandler>();

        public Task Handle(OrderCompleted message, IMessageHandlerContext context)
        {
            Log.Info("Handle OrderCompleted");

            return context.Publish(new OrderBilled {OrderId = message.OrderId});
        }
    }
}