using AutoMapper;
using PixelMart.API.Entities;
using PixelMart.API.Models.Category;

namespace PixelMart.API.Profiles;

public class CategoriesProfile : Profile
{
    public CategoriesProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<CategoryForCreationDto, Category>();
        CreateMap<CategoryForUpdateDto, Category>();
    }
}
