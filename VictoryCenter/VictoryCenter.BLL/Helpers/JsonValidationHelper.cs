using System.Text.Json;

namespace VictoryCenter.BLL.Helpers;

public static class JsonValidationHelper
{
    public static List<string> ValidateJsonAgainstType(string json, Type targetType, JsonSerializerOptions options)
    {
        var errors = new List<string>();

        try
        {
            var jsonDocument = JsonDocument.Parse(json);
            var jsonPropertyNames = new List<string>();

            foreach (var property in jsonDocument.RootElement.EnumerateObject())
            {
                jsonPropertyNames.Add(property.Name);
            }

            var targetPropertyNames = new List<string>();
            foreach (var property in targetType.GetProperties())
            {
                targetPropertyNames.Add(property.Name);
            }

            foreach (var jsonProperty in jsonPropertyNames)
            {
                var propertyExists = false;

                foreach (var targetProperty in targetPropertyNames)
                {
                    if (string.Equals(jsonProperty, targetProperty, StringComparison.OrdinalIgnoreCase))
                    {
                        propertyExists = true;
                        break;
                    }
                }

                if (!propertyExists)
                {
                    errors.Add($"Unknown property '{jsonProperty}' is not allowed");
                }
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            JsonSerializer.Deserialize(json, targetType, options);
        }
        catch (JsonException ex)
        {
            errors.Add(FormatJsonError(ex.Message));
        }

        return errors;
    }

    public static string FormatJsonError(string rawError)
    {
        if (rawError.Contains("could not be mapped to any .NET member"))
        {
            var startIndex = rawError.IndexOf("'") + 1;
            var endIndex = rawError.IndexOf("'", startIndex);

            if (startIndex > 0 && endIndex > startIndex)
            {
                var propertyName = rawError.Substring(startIndex, endIndex - startIndex);
                return $"Unknown property '{propertyName}' is not allowed";
            }
        }

        if (rawError.Contains("Comments are not allowed"))
        {
            return "JSON comments are not allowed";
        }

        if (rawError.Contains("Trailing commas are not allowed"))
        {
            return "Trailing commas are not allowed in JSON";
        }

        return rawError;
    }
}
