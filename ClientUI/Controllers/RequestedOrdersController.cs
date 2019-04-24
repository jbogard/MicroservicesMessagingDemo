using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ClientUI.Data;
using ClientUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using Sales.Messages;


namespace ClientUI.Controllers
{
    public class RequestedOrdersController : Controller
    {
        private readonly IEndpointInstance _endpointInstance;
        private readonly StoreDbContext _dbContext;
        private readonly IMapper _mapper;

        public RequestedOrdersController(IEndpointInstance endpointInstance, StoreDbContext dbContext, IMapper mapper)
        {
            _endpointInstance = endpointInstance;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        // GET
        public IActionResult Index()
        {
            var items = _dbContext.PlacedOrderRequests.ToArray();

            var viewModel = new RequestedOrdersPage
            { PlacedOrderRequests = items };


            return View(viewModel);
        }


        public async Task<IActionResult> ViewOrderRequest(string orderId)
        {
            var order = await _dbContext.PlacedOrderRequests.Where(x => x.OrderId == orderId).FirstAsync();
            var orderRequest = _mapper.Map<UpdateOrderCommand>(order);
            return View(orderRequest);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderRequest(UpdateOrderCommand command)
        {

            var order = await _dbContext.PlacedOrderRequests.Where(x => x.OrderId == command.OrderId).FirstAsync();
            if (order.ColorRequired == true && command.ColorSelected == null)
            {
                return View("ViewOrderRequest", command);
            }

            order.ColorSelected = command.ColorSelected;
            order.OrderStatus = OrderStatus.Submitted;
            await _dbContext.SaveChangesAsync();

            var submitCommand = new PlaceOrder
            {
                OrderId = order.OrderId,
                OrderDate = order.Date,
                ItemName = order.ItemName,
                Color = command.ColorSelected
            };

            // Send the command to submit the order
            await _endpointInstance.Send(submitCommand)
               .ConfigureAwait(false);

            return RedirectToAction("Index");
        }
    }

    public class RequestedOrdersPage
    {
        public PlacedOrderRequest[] PlacedOrderRequests { get; set; }
        public string ColorChosen { get; set; }
    }

    public class UpdateOrderCommand
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public string OrderId { get; set; }

        public string ItemName { get; set; }

        public OrderStatus OrderStatus { get; set; }
        public bool? ColorRequired { get; set; }
        public string ColorOptions { get; set; }
        public string ColorSelected { get; set; }
    }
}