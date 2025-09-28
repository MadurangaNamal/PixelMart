using PixelMart.API.Helpers;

namespace PixelMart.API.Services.Impl;

/// <summary>
/// Provides a mapping between source and destination properties for object-to-object mapping scenarios.
/// Used to define how properties from a source type correspond to properties in a destination type, 
/// supporting flexible mapping logic.
/// </summary>
public class PropertyMapping<TSource, TDestination> : IPropertyMapping
{
    public Dictionary<string, PropertyMappingValue> MappingDictionary { get; private set; }

    public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
    {
        MappingDictionary = mappingDictionary ??
            throw new ArgumentNullException(nameof(mappingDictionary));
    }
}
