using AutoMapper;
using PixelMart.API.Models.Product;

namespace PixelMart.API.Profiles;

public class ProductsProfile : Profile
{
    public ProductsProfile()
    {
        CreateMap<Entities.Product, ProductDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.Brand} {src.Name}"));

        CreateMap<ProductForCreationDto, Entities.Product>();

        CreateMap<ProductForUpdateDto, Entities.Product>().ReverseMap();
    }
}
