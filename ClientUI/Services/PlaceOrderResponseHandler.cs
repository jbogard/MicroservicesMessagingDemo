﻿using System.Linq;
using System.Threading.Tasks;
using ClientUI.Data;
using ClientUI.Models;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using Sales.Messages;

namespace ClientUI.Services
{
    public class PlaceOrderResponseHandler : IHandleMessages<PlaceOrderResponse>
    {
        private readonly StoreDbContext _dbContext;

        public PlaceOrderResponseHandler(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(PlaceOrderResponse message, IMessageHandlerContext context)
        {
            var originalRequest = await _dbContext
                .PlacedOrderRequests
                .Where(x => x.OrderId == message.OrderId)
                .FirstAsync();

            if (message.StatusCompleted)
            {
                originalRequest.OrderStatus = OrderStatus.Received;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
