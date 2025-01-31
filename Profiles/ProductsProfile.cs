using AutoMapper;

namespace PixelMart.API.Profiles;

public class ProductsProfile : Profile
{
    public ProductsProfile()
    {
        CreateMap<Entities.Product, Models.ProductDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.Brand} {src.Name}"));

        CreateMap<Models.ProductForCreationDto, Entities.Product>();

        CreateMap<Models.ProductForUpdateDto, Entities.Product>().ReverseMap();
    }
}
