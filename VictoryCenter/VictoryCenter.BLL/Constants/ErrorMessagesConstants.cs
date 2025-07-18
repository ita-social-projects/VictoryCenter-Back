namespace VictoryCenter.BLL.Constants;

public static class ErrorMessagesConstants
{
    public static readonly string UnknownStatusValue = "Unknown status value";

    public static string NotFound()
    {
        return "Not Found";
    }

    public static string NotFound(object? id, Type entityType)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        return $"Entity {entityType.Name} with id '{id}' was not found";
    }

    public static string FailedToCreateEntity(Type entityType)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        return $"Failed to create new {entityType.Name}";
    }

    public static string FailedToCreateEntityInTheDatabase(Type entityType)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        return $"Failed to create new {entityType.Name} in database";
    }

    public static string FailedToUpdateEntity(Type entityType)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        return $"Failed to update {entityType.Name}";
    }

    public static string FailedToDeleteEntity(Type entityType)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        return $"Failed to delete {entityType.Name}";
    }

    public static string ReorderingContainsInvalidIds(Type entityType, IEnumerable<long> ids)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        return $"Invalid IDs found while reodering {entityType.Name}: {string.Join(", ", ids)}";
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

    public static string CollectionCannotBeEmpty(string collection)
    {
        return $"{collection} cannot be empty";
    }

    public static string CollectionCannotContainMoreThan(string collection, long numberOfElements)
    {
        return $"{collection} cannot contain more than {numberOfElements} elements";
    }

    public static string CollectionContainsDuplicateValues(string collection)
    {
        return $"{collection} must contain unique values";
    }
}
