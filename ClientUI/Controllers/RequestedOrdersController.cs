using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ClientUI.Data;
using ClientUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ClientUI.Controllers
{
    public class RequestedOrdersController : Controller
    {
        private readonly StoreDbContext _dbContext;
        private readonly IMapper _mapper;

        public RequestedOrdersController(StoreDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        // GET
        public IActionResult Index()
        {
            var items = _dbContext.PlacedOrderRequests.ToArray();

            var viewModel = new RequestedOrdersPage
            {
                PlacedOrderRequests = items
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(string orderId)
        {
            var order = await _dbContext.PlacedOrderRequests
                .Where(x => x.OrderId == orderId)
                .ProjectTo<RequestedOrdersDetailsModel>(_mapper.ConfigurationProvider)
                .FirstAsync();

            return View(order);
        }
    }

    public class RequestedOrdersPage
    {
        public PlacedOrderRequest[] PlacedOrderRequests { get; set; }
    }

    public class RequestedOrdersDetailsModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public string OrderId { get; set; }

        public string ItemName { get; set; }

        public OrderStatus OrderStatus { get; set; }
    }
}