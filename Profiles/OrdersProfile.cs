using AutoMapper;
using PixelMart.API.Entities;
using PixelMart.API.Models.Order;

namespace PixelMart.API.Profiles;

public class OrdersProfile : Profile
{
    public OrdersProfile()
    {
        CreateMap<OrderManipulationDto, Order>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.OrderStatus));

        CreateMap<OrderUpdateDto, Order>()
            .IncludeBase<OrderManipulationDto, Order>();

        CreateMap<OrderCreationDto, Order>()
            .IncludeBase<OrderManipulationDto, Order>();

        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Status))
            .ReverseMap();
    }
}