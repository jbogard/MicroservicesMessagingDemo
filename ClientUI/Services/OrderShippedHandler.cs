using System.Linq;
using System.Threading.Tasks;
using ClientUI.Data;
using ClientUI.Models;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using Shipping.Events;

namespace ClientUI.Services
{
    public class OrderShippedHandler : IHandleMessages<OrderShipped>
    {
        private readonly StoreDbContext _dbContext;

        public OrderShippedHandler(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(OrderShipped message, IMessageHandlerContext context)
        {
            var originalRequest = await _dbContext
                .PlacedOrderRequests
                .Where(x => x.OrderId == message.OrderId)
                .FirstAsync();

            originalRequest.OrderStatus = OrderStatus.Shipped;

            await _dbContext.SaveChangesAsync();
        }
    }
}
