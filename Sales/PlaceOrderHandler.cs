using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using NServiceBus.Logging;
using Sales.Data;
using Sales.Events;
using Sales.Messages;
using Sales.Models;

namespace Sales
{
    public class PlaceOrderHandler :
        IHandleMessages<PlaceOrder>
    {
        private readonly OrdersDbContext _dbContext;
        static readonly ILog Log = LogManager.GetLogger<PlaceOrderHandler>();

        public PlaceOrderHandler(OrdersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(PlaceOrder message, IMessageHandlerContext context)
        {
            Log.Info($"Received PlaceOrder, OrderId = {message.OrderId}, " +
                     $"OrderDate = {message.OrderDate}, ItemName = {message.ItemName}");

            var order = new SubmittedOrder {OrderDate = message.OrderDate, OrderId = message.OrderId};

            await _dbContext.SubmittedOrders.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            var orderResponse = new PlaceOrderResponse
            {
                OrderId = message.OrderId,
                StatusCompleted = true
            };

            await context.Reply(orderResponse);

            var orderCompleted = new OrderCompleted
            {
                OrderId = message.OrderId
            };

            await context.Publish(orderCompleted);
        }
    }
}
