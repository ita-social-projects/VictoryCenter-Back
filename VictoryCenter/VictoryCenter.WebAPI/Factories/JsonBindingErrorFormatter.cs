using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Abstractions;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.WebAPI.Factories;

internal static class JsonBindingErrorFormatter
{
    private const string JsonPathPrefix = "$";
    private const string JsonPropertyPathPrefix = "$.";
    private const string UnknownBodyName = "RequestBody";

    private static readonly HashSet<Type> NumericTypes =
    [
        typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint),
        typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal), typeof(Half),
    ];

    public static void Format(IDictionary<string, string[]> errors, ParameterDescriptor? bodyParameter)
    {
        var jsonPaths = errors.Keys.Where(key => key.StartsWith(JsonPathPrefix, StringComparison.Ordinal)).ToList();
        if (jsonPaths.Count == 0)
        {
            return;
        }

        if (bodyParameter is not null)
        {
            errors.Remove(bodyParameter.Name);
        }

        var bodyType = bodyParameter?.ParameterType;
        var bodyName = bodyType?.Name;

        foreach (var jsonPath in jsonPaths)
        {
            errors.Remove(jsonPath);

            var segments = GetPathSegments(jsonPath);
            if (segments.Count == 0)
            {
                var bodyKey = bodyName ?? UnknownBodyName;
                AddError(errors, bodyKey, ErrorMessagesConstants.PropertyMustBeInAValidFormat(bodyKey));
                continue;
            }

            var propertyName = string.Join('.', segments.Select(ToPascalCase));
            var key = bodyName is null ? propertyName : $"{bodyName}.{propertyName}";
            var message = IsNumeric(ResolvePropertyType(bodyType, segments))
                ? ErrorMessagesConstants.PropertyMustContainOnlyDigits(propertyName)
                : ErrorMessagesConstants.PropertyMustBeInAValidFormat(propertyName);

            AddError(errors, key, message);
        }
    }

    private static List<string> GetPathSegments(string jsonPath) =>
        jsonPath.StartsWith(JsonPropertyPathPrefix, StringComparison.Ordinal)
            ? [.. jsonPath[JsonPropertyPathPrefix.Length..].Split('.').Where(segment => segment.Length > 0)]
            : [];

    private static Type? ResolvePropertyType(Type? rootType, IEnumerable<string> segments)
    {
        var currentType = rootType;

        foreach (var segment in segments)
        {
            if (currentType is null)
            {
                return null;
            }

            var bracketIndex = segment.IndexOf('[', StringComparison.Ordinal);
            var name = bracketIndex >= 0 ? segment[..bracketIndex] : segment;

            currentType = FindProperty(currentType, name)?.PropertyType;

            if (currentType is not null && bracketIndex >= 0)
            {
                currentType = GetElementType(currentType);
            }
        }

        return currentType;
    }

    private static PropertyInfo? FindProperty(Type type, string jsonName) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(property => string.Equals(
                property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? property.Name,
                jsonName,
                StringComparison.OrdinalIgnoreCase));

    private static Type? GetElementType(Type type)
    {
        if (type.IsArray)
        {
            return type.GetElementType();
        }

        return type.IsGenericType ? type.GetGenericArguments().FirstOrDefault() : null;
    }

    private static bool IsNumeric(Type? type) =>
        type is not null && NumericTypes.Contains(Nullable.GetUnderlyingType(type) ?? type);

    private static void AddError(IDictionary<string, string[]> errors, string key, string message) =>
        errors[key] = errors.TryGetValue(key, out var existing)
            ? [.. existing.Append(message).Distinct()]
            : [message];

    private static string ToPascalCase(string segment) =>
        char.ToUpperInvariant(segment[0]) + segment[1..];
}
