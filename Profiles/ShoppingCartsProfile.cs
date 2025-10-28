using AutoMapper;
using PixelMart.API.Entities;
using PixelMart.API.Models.ShoppingCart;

namespace PixelMart.API.Profiles;

public class ShoppingCartsProfile : Profile
{
    public ShoppingCartsProfile()
    {
        CreateMap<ShoppingCart, ShoppingCartManipulationDto>();
        CreateMap<ShoppingCart, ShoppingCartDto>().ReverseMap();
        CreateMap<CartItem, CartItemDto>().ReverseMap();
        CreateMap<ShoppingCartManipulationDto, ShoppingCart>();

        CreateMap<ShoppingCart, ShoppingCartUpdateDto>()
            .IncludeBase<ShoppingCart, ShoppingCartManipulationDto>();

        CreateMap<ShoppingCartUpdateDto, ShoppingCart>()
            .IncludeBase<ShoppingCartManipulationDto, ShoppingCart>();
    }
}
