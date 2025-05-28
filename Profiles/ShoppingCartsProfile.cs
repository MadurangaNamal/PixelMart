using AutoMapper;
using PixelMart.API.Entities;
using PixelMart.API.Models.ShoppingCart;

namespace PixelMart.API.Profiles;

public class ShoppingCartsProfile : Profile
{
    public ShoppingCartsProfile()
    {
        CreateMap<ShoppingCart, ShoppingCartManipulationDto>();
        CreateMap<ShoppingCart, ShoppingCartUpdateDto>()
            .IncludeBase<ShoppingCart, ShoppingCartManipulationDto>();

        CreateMap<ShoppingCartManipulationDto, ShoppingCart>();
        CreateMap<ShoppingCartUpdateDto, ShoppingCart>()
            .IncludeBase<ShoppingCartManipulationDto, ShoppingCart>();

        CreateMap<ShoppingCart, ShoppingCartDto>().ReverseMap();
        CreateMap<CartItem, CartItemDto>().ReverseMap();
    }
}
