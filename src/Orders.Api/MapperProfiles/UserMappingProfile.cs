using AutoMapper;
using Orders.Api.DbModels;
using Orders.Api.Models.DataTransferObjects;

namespace Orders.Api.MapperProfiles;
public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderDto>();
        CreateMap<CreateOrderDto, Order>();
    }
}