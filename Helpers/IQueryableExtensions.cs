using System.Linq.Dynamic.Core;

namespace PixelMart.API.Helpers;

/// <summary>
/// Provides extension methods for IQueryable to apply sorting based on a given orderBy string 
/// </summary>
public static class IQueryableExtensions
{
    public static IQueryable<T> ApplySort<T>(this IQueryable<T> source,
        string orderBy,
        Dictionary<string, PropertyMappingValue> mappingDictionary)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(mappingDictionary);

        if (string.IsNullOrWhiteSpace(orderBy))
            return source;

        var orderByParts = orderBy
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .SelectMany(clause => BuildOrderByParts(clause, mappingDictionary));

        var orderByString = string.Join(", ", orderByParts);

        return string.IsNullOrEmpty(orderByString)
            ? source
            : source.OrderBy(orderByString);
    }

    private static IEnumerable<string> BuildOrderByParts(
        string clause,
        Dictionary<string, PropertyMappingValue> mappingDictionary)
    {
        var (propertyName, descending) = ParseClause(clause);

        if (!mappingDictionary.TryGetValue(propertyName, out var mapping) || mapping is null)
            throw new ArgumentException($"Key mapping for '{propertyName}' is missing", nameof(mappingDictionary));

        if (mapping.Revert)
            descending = !descending;

        var direction = descending ? "descending" : "ascending";

        return mapping.DestinationProperties.Select(property => $"{property} {direction}");
    }

    private static (string PropertyName, bool Descending) ParseClause(string clause)
    {
        var parts = clause.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var descending = parts.Length > 1 &&
                         parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

        return (parts[0], descending);
    }
}

