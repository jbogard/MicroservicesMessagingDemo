using NServiceBus;

namespace Sales.Messages
{
    public class PlaceOrderResponse : IMessage
    {
        public string OrderId { get; set; }
        public bool NeedsColor { get; set; }
        public bool StatusCompleted { get; set; }
        public string[] ColorChoices { get; set; }
    }
}
