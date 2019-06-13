using NServiceBus;

namespace Shipping.Events
{
    public class OrderShipped : IEvent
    {
        public string OrderId { get; set; }
    }
}