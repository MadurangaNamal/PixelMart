using AutoMapper;

namespace PixelMart.API.Profiles;

public class CategoriesProfile : Profile
{
    public CategoriesProfile()
    {
        CreateMap<Entities.Category, Models.CategoryDto>();
    }
}
