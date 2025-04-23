using PixelMart.API.Helpers;

namespace PixelMart.API.Services;

public interface IPropertyMappingService
{
    Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
    bool ValidMappingExistsFor<TSource, TDestination>(string fields);
}
