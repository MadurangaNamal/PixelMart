using AutoMapper;
using PixelMart.API.Entities;
using PixelMart.API.Models.ShoppingCart;

namespace PixelMart.API.Profiles;

public class ShoppingCartsProfile : Profile
{
    public ShoppingCartsProfile()
    {
        CreateMap<ShoppingCartManipulationDto, ShoppingCart>();

        CreateMap<ShoppingCart, ShoppingCartDto>().ReverseMap();

        CreateMap<CartItem, CartItemDto>().ReverseMap();

    }
}
