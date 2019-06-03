using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClientUI.Data;
using Microsoft.AspNetCore.Mvc;
using ClientUI.Models;
using NServiceBus;
using Sales.Messages;

namespace ClientUI.Controllers
{
    public class HomeController : Controller
    {
        IEndpointInstance _endpointInstance;
        private readonly StoreDbContext _dbcontext;
        static int messagesSent;

        public HomeController(IEndpointInstance endpointInstance, StoreDbContext dbcontext)
        {
            _endpointInstance = endpointInstance;
            _dbcontext = dbcontext;
        }

        public IActionResult Index()
        {
            return View();
        }

 
        [HttpPost]
        public async Task<IActionResult> PlaceSpatulaOrder()
        {
            var model = await SubmitOrder("Spatula");

            return PartialView("PlaceSpatulaOrderPartial", model);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOvenMittOrder()
        {
            var model = await SubmitOrder("ovenmitt");

            return PartialView("PlaceOvenMittOrderPartial", model);
        }

        private async Task<dynamic> SubmitOrder(string productName)
        {
            var placeOrderRequest = await SaveOrderRequest(productName);

            await SendPlaceOrderCommand(productName, placeOrderRequest);

            var model = BuildResponseModel(placeOrderRequest);

            return model;
        }

        private async Task<PlacedOrderRequest> SaveOrderRequest(string productName)
        {
            var orderId = Guid.NewGuid().ToString().Substring(0, 8);

            var placeOrderRequest = new PlacedOrderRequest
            {
                Date = DateTime.Now,
                OrderId = orderId,
                OrderStatus = OrderStatus.Submitted,
                ItemName = productName
            };

            await _dbcontext.PlacedOrderRequests.AddAsync(placeOrderRequest);
            await _dbcontext.SaveChangesAsync();

            return placeOrderRequest;
        }

        private async Task SendPlaceOrderCommand(string productName, PlacedOrderRequest placeOrderRequest)
        {
            var command = new PlaceOrder
            {
                OrderId = placeOrderRequest.OrderId,
                OrderDate = placeOrderRequest.Date,
                ItemName = productName
            };

            // Send the command to submit the order
            await _endpointInstance.Send(command)
                .ConfigureAwait(false);
        }

        private static dynamic BuildResponseModel(PlacedOrderRequest placeOrderRequest)
        {
            dynamic model = new ExpandoObject();
            model.OrderId = placeOrderRequest.OrderId;
            model.MessagesSent = Interlocked.Increment(ref messagesSent);
            model.OrderSubmitted = true;
            return model;
        }
    }
}
