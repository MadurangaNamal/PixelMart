using PixelMart.API.Entities;
using PixelMart.API.Helpers;
using PixelMart.API.Models.Product;

namespace PixelMart.API.Services.Impl;

/// <summary>
/// Service for managing property mappings between source and destination types.
/// Enables flexible object-to-object mapping and validation of property mapping configurations, supporting dynamic field selection and sorting in APIs.
/// </summary>
public class PropertyMappingService : IPropertyMappingService
{
    private readonly Dictionary<string, PropertyMappingValue> _productPropertyMapping = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", new(["Id"]) },
            { "Name", new(["Name"]) },
            { "FullName", new(["Brand","Name"]) }
        };

    // list of property mappings
    private readonly IList<IPropertyMapping> _propertyMappings = [];

    public PropertyMappingService()
    {
        _propertyMappings.Add(new PropertyMapping<ProductDto, Product>(_productPropertyMapping));
    }

    // Get the property mapping dictionary
    public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
    {

        // get matching mapping
        var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

        if (matchingMapping.Count() == 1)
        {
            return matchingMapping.First().MappingDictionary;
        }

        throw new Exception($"Cannot find exact property mapping instance " + $"for <{typeof(TSource)},{typeof(TDestination)}");
    }

    // Validates if a valid mapping exists for the given fields
    public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
    {
        var propertyMapping = GetPropertyMapping<TSource, TDestination>();

        if (string.IsNullOrWhiteSpace(fields))
        {
            return true;
        }

        var fieldsAfterSplit = fields.Split(',');

        foreach (var field in fieldsAfterSplit)
        {
            var trimmedField = field.Trim();

            // remove everything after the first " " 
            var indexOfFirstSpace = trimmedField.IndexOf(" ");
            var propertyName = indexOfFirstSpace == -1 ? trimmedField : trimmedField.Remove(indexOfFirstSpace);

            // find the matching property
            if (!propertyMapping.ContainsKey(propertyName))
            {
                return false;
            }
        }

        return true;
    }
}
