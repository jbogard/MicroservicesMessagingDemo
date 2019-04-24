using NServiceBus;

namespace Sales.Messages
{
    public class CompleteOrder : ICommand
    {
        public string OrderId { get; set; }
    }
}