using System;
using System.Linq;
using System.Threading.Tasks;
using ClientUI.Data;
using ClientUI.Hubs;
using ClientUI.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using Sales.Messages;

namespace ClientUI.Services
{
    public class PlaceOrderResponseHandler : IHandleMessages<PlaceOrderResponse>
    {
        private readonly StoreDbContext _dbContext;
        Lazy<IHubContext<SubmissionNotificationHub>> lazyHub;

        public PlaceOrderResponseHandler(StoreDbContext dbContext, Lazy<IHubContext<SubmissionNotificationHub>> lazyHub)
        {
            _dbContext = dbContext;
            this.lazyHub = lazyHub;
        }

        public async Task Handle(PlaceOrderResponse message, IMessageHandlerContext context)
        {
            var originalRequest = await _dbContext.PlacedOrderRequests.Where(x => x.OrderId == message.OrderId)
                .FirstAsync();

            var notificationMessage = "";

            if (message.StatusCompleted)
            {
                originalRequest.OrderStatus = OrderStatus.Received;
                notificationMessage = "Order " + message.OrderId + " Received";
            }
            else 
            {
                originalRequest.OrderStatus = OrderStatus.NeedsMoreInfo;
                if (message.NeedsColor)
                {
                    originalRequest.ColorRequired = true;
                    originalRequest.ColorOptions = string.Join(",", message.ColorChoices);

                    notificationMessage = "Order " + message.OrderId + " needs a color choice";
                }
            }
            await _dbContext.SaveChangesAsync();

            await lazyHub.Value.Clients.All.SendAsync("ReceiveMessage", notificationMessage);


        }
    }
}
