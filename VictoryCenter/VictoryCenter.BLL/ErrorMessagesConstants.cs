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
}
