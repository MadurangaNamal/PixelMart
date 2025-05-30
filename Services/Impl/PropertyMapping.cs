﻿using PixelMart.API.Helpers;

namespace PixelMart.API.Services.Impl;

public class PropertyMapping<TSource, TDestination> : IPropertyMapping
{
    public Dictionary<string, PropertyMappingValue> MappingDictionary { get; private set; }

    public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
    {
        MappingDictionary = mappingDictionary ??
            throw new ArgumentNullException(nameof(mappingDictionary));
    }
}
