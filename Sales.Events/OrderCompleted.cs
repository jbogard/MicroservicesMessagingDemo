using NServiceBus;

namespace Sales.Events
{
    public class OrderCompleted : IEvent
    {
        public string OrderId { get; set; }
    }
}