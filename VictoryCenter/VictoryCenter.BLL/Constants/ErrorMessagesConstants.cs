using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Constants;

public static class ErrorMessagesConstants
{
    public static readonly string UnknownStatusValue = "Unknown status value";
    public static readonly string OnlyDigitsExpression = "^[0-9]+$";

    public static string NotFound()
    {
        return "Not Found";
    }

    public static string NotFound(IEnumerable<long> ids, Type entityType)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        return $"Entities {entityType.Name} with id(s) '{string.Join(", ", ids)}' was not found";
    }

    public static string NotFound(object? id, Type entityType)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        return $"Entity {entityType.Name} with id '{id}' was not found";
    }

    public static string NotFoundByIdentifier(object? identifier, Type entityType)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        return $"Entity {entityType.Name} with identifier '{identifier}' was not found";
    }

    public static string CannotUpdateAndDeleteSameEntity(IEnumerable<long> ids, Type entityType)
    {
        return $"Cannot update and delete the same entity {entityType.Name}. Conflicting IDs: {string.Join(", ", ids)}";
    }

    public static string OnlyOneEntityOfTypeIsAllowed(string entityTypeName)
    {
        return $"Only one instance of {entityTypeName} is allowed";
    }

    public static string FailedToCreateEntity(Type entityType)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        return $"Failed to create new {entityType.Name}";
    }

    public static string FailedToCreateEntityInDatabase(Type entityType)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        return $"Failed to create new {entityType.Name} in the database";
    }

    public static string FailedToUpdateEntity(Type entityType)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        return $"Failed to update {entityType.Name}";
    }

    public static string FailedToUpdateEntityInDatabase(Type entityType)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        return $"Failed to update {entityType.Name} in the database";
    }

    public static string FailedToDeleteEntity(Type entityType)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        return $"Failed to delete {entityType.Name}";
    }

    public static string FailedToDeleteEntities(Type entityType)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        return $"Failed to delete {entityType.Name} entities";
    }

    public static string FailedToDeleteEntitiesInDatabase(Type entityType)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        return $"Failed to delete {entityType.Name} entities in the database";
    }

    public static string FailedToDeleteEntityInDatabase(Type entityType)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        return $"Failed to delete {entityType.Name} in the database";
    }

    public static string ReorderingContainsInvalidIds(Type entityType, IEnumerable<long> ids)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        return $"Invalid IDs found while reordering {entityType.Name}: {string.Join(", ", ids)}";
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

    public static string PropertyMustBeUnique(string property)
    {
        return $"{property} must be unique";
    }

    public static string CollectionCannotBeEmpty(string collection)
    {
        return $"{collection} cannot be empty";
    }

    public static string CollectionCannotContainMoreThan(string collection, long numberOfElements)
    {
        return $"{collection} cannot contain more than {numberOfElements} elements";
    }

    public static string CollectionMustContainUniqueValues(string collection)
    {
        return $"{collection} must contain unique values";
    }

    public static string CollectionCannotContainNullElements(string collectionName)
    {
        return $"{collectionName} cannot contain null elements";
    }

    public static string PropertyMustHaveALengthOfNCharacters(string property, int length)
    {
        return $"{property} must have a length of {length} characters";
    }

    public static string PropertyMustBeValidEnum(string property)
    {
        return $"{property} must be a valid value";
    }

    public static string PropertyNotAllowedForContentType(string property, ContentType contentType)
    {
        return $"{property} is not allowed for content type {contentType}";
    }

    public static string BlobStorageError(string message)
    {
        return $"Blob error: {message}";
    }

    public static string FailedToRetrieveImage()
    {
        return "Failed to retrieve image from storage";
    }

    public static string FailedToRetrievePdf()
    {
        return "Failed to retrieve PDF from storage";
    }

    public static string PropertyMustContainOnlyDigits(string property)
    {
        return $"{property} must contain only digits";
    }

    public static string PropertyMustBeGreaterThanOrEqualToN(string property, long value)
    {
        return $"{property} must be greater than or equal to {value}";
    }

    public static string PropertyMustBeLessThanOrEqualToN(string property, long value)
    {
        return $"{property} must be less than or equal to {value}";
    }

    public static string MetricsMustContainExactlyNItems(int count)
    {
        return $"Metrics must contain exactly {count} items.";
    }

    public static string MetricTypesMustBeUnique()
    {
        return "Each of the four metric types must appear exactly once: Partners, Programs, Raised, TherapyHours.";
    }

    public static string LocalizationOnlyAllowedForRaisedMetric()
    {
        return "Localization is only allowed for the metric with type Raised.";
    }
}
