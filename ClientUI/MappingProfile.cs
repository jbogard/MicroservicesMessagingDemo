using AutoMapper;
using ClientUI.Controllers;
using ClientUI.Models;

namespace ClientUI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PlacedOrderRequest, RequestedOrdersDetailsModel>();
        }
    }
}