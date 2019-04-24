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
                     $"OrderDate = {message.OrderDate}, ItemName = {message.ItemName}, Color={message.Color}");


            var orderResponse = new PlaceOrderResponse {OrderId = message.OrderId};
            var order = new SubmittedOrder { OrderDate = message.OrderDate, OrderId = message.OrderId };
            if (message.ItemName == "ovenmitt")
            {
                if (message.Color == null)
                {
                    await PopulateMissingColorResponse(orderResponse);
                    await context.Reply(orderResponse);
                    return;
                }

                var selectedColor = await _dbContext.Colors.Where(x => x.Name == message.Color).FirstAsync();
                order.Color = selectedColor;
                await _dbContext.SubmittedOrders.AddAsync(order);
            }

            await _dbContext.SubmittedOrders.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            orderResponse.StatusCompleted = true;


            await context.Reply(orderResponse);
            await context.Publish(new OrderCompleted {OrderId = message.OrderId});
        }

        private async Task PopulateMissingColorResponse(PlaceOrderResponse orderResponse)
        {
            orderResponse.NeedsColor = true;
            orderResponse.StatusCompleted = false;

            orderResponse.ColorChoices = await _dbContext.Colors.Select(x => x.Name).ToArrayAsync(); ;
        }
    }
}
