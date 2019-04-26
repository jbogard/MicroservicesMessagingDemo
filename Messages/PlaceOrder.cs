using System;
using NServiceBus;

namespace Sales.Messages
{
    public class PlaceOrder : ICommand
    {
        public string OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string ItemName { get; set; }
    }
}
