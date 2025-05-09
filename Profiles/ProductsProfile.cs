using AutoMapper;
using PixelMart.API.Entities;
using PixelMart.API.Models.Product;

namespace PixelMart.API.Profiles;

public class ProductsProfile : Profile
{
    public ProductsProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.Brand} {src.Name}"))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Stock!.Quantity))
            .ForMember(dest => dest.IsLowStock, opt => opt.MapFrom(src => src.Stock!.IsLowStock));

        CreateMap<ProductForCreationDto, Product>();

        CreateMap<ProductForUpdateDto, Product>().ReverseMap();
    }
}
