using AutoMapper;
using PixelMart.API.Entities;
using PixelMart.API.Models.Order;

namespace PixelMart.API.Profiles;

public class OrdersProfile : Profile
{
    public OrdersProfile()
    {
        CreateMap<Order, OrderManipulationDto>();
        CreateMap<Order, OrderUpdateDto>()
            .IncludeBase<Order, OrderManipulationDto>();

        CreateMap<OrderManipulationDto, Order>();
        CreateMap<OrderUpdateDto, Order>()
            .IncludeBase<OrderManipulationDto, Order>();

        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
            .ReverseMap();
    }
}