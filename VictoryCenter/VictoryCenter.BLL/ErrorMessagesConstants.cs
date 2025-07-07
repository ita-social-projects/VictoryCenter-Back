namespace VictoryCenter.BLL;

public static class ErrorMessagesConstants
{
    public static string NotFound()
    {
        return "Not Found";
    }

    public static string NotFound(object? id, Type entityType)
    {
        return $"Entity {entityType.Name} with id '{id}' was not found";
    }

    public static string PropertyMustHaveAMinimumLenghtOfNCharacters(string property, int lenght)
    {
        return $"{property} field must have a minimum lenght of {lenght} characters";
    }

    public static string PropertyMustHaveAMaximumLenghtOfNCharacters(string property, int lenght)
    {
        return $"{property} field must have a maximum lenght of {lenght} characters";
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
}
