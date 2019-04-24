using System.Threading.Tasks;
using Billing.Events;
using NServiceBus;
using NServiceBus.Logging;
using Sales.Events;

namespace Shipping
{

    public class ShippingSagaData : ContainSagaData
    {
        public string OrderId { get; set; }
        public bool IsOrderCompleted { get; set; }
        public bool IsOrderBilled { get; set; }
    }

    public class ShippingSaga : Saga<ShippingSagaData>,
        IAmStartedByMessages<OrderCompleted>,
        IAmStartedByMessages<OrderBilled>
    {
        private static readonly ILog Log = LogManager.GetLogger<ShippingSaga>();

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShippingSagaData> mapper)
        {
            mapper.ConfigureMapping<OrderCompleted>(m => m.OrderId).ToSaga(s => s.OrderId);
            mapper.ConfigureMapping<OrderBilled>(m => m.OrderId).ToSaga(s => s.OrderId);
        }

        public Task Handle(OrderCompleted message, IMessageHandlerContext context)
        {
            Log.Info("Handle OrderCompleted");

            Data.IsOrderCompleted = true;

            return ProcessOrder();
        }

        public Task Handle(OrderBilled message, IMessageHandlerContext context)
        {
            Log.Info("Handle OrderBilled");

            Data.IsOrderBilled = true;

            return ProcessOrder();
        }

        private Task ProcessOrder()
        {
            if (Data.IsOrderCompleted && Data.IsOrderBilled)
            {
                Log.Info($"Shipping Order {Data.OrderId}");
                MarkAsComplete();
            }

            return Task.CompletedTask;
        }
    }
}