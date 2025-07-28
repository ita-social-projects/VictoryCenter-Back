namespace VictoryCenter.BLL.Constants;

public static class ErrorMessagesConstants
{
    public static string NotFound()
    {
        return "Not Found";
    }

    public static string NotFound(object? id, Type entityType)
    {
        if (entityType == null)
        {
            throw new ArgumentNullException(nameof(entityType));
        }

        return $"Entity {entityType.Name} with id '{id}' was not found";
    }

    public static string PropertyMustHaveAMinimumLengthOfNCharacters(string property, int length)
    {
        return $"{property} field must have a minimum length of {length} characters";
    }

    public static string PropertyMustHaveAMaximumLengthOfNCharacters(string property, int length)
    {
        return $"{property} field must have a maximum length of {length} characters";
    }

    public static string PropertyMustBeGreaterThan(string property, int value)
    {
        return $"{property} must be greater than {value}";
    }

    public static string PropertyIsRequired(string property)
    {
        return $"{property} is required";
    }

    public static string PropertyMustBePositive(string property)
    {
        return $"{property} must be positive";
    }

    public static string PropertyMustBeInAValidFormat(string property, string? format = null)
    {
        return format is null
            ? $"{property} must be in a valid format"
            : $"{property} must be in a valid format of {format}";
    }

    public static string BlobStorageError(string message)
    {
        return $"Blob error: {message}";
    }
}
