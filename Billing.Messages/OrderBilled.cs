using NServiceBus;

namespace Billing.Events
{
    public class OrderBilled : IEvent
    {
        public string OrderId { get; set; }
    }
}