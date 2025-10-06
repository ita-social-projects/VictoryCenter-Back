namespace VictoryCenter.BLL.Constants;

public static class WhoWeAreConstants
{
    public static string WrongContentType => "Content has wrong content type";
    public static string ContentCanNotBeNull => "Content cannot be null";
    public static string EntityDoNotBelongToTheSection(Type entity, long sectionId)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        return $"Entity {entity.Name} does not belong to the section with id {sectionId}";
    }
}
