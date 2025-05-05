using AutoMapper;
using PixelMart.API.Models.Category;

namespace PixelMart.API.Profiles;

public class CategoriesProfile : Profile
{
    public CategoriesProfile()
    {
        CreateMap<Entities.Category, CategoryDto>().ReverseMap();

        CreateMap<CategoryForCreationDto, Entities.Category>();

        CreateMap<CategoryForUpdateDto, Entities.Category>();
    }
}
