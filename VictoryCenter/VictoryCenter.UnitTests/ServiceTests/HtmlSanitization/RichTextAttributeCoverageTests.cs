using System.Reflection;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;

namespace VictoryCenter.UnitTests.ServiceTests.HtmlSanitization;

public class RichTextAttributeCoverageTests
{
    [Fact]
    public void LocalizationProperties_OfRichTextProperties_ShouldAlsoBeMarkedAsRichText()
    {
        // Arrange
        var translatedEntityTypes = typeof(ITranslatedEntity<>).Assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Select(t => (EntityType: t, LocalizationType: GetLocalizationType(t)))
            .Where(x => x.LocalizationType is not null)
            .ToList();

        // Act
        var unmarkedProperties = translatedEntityTypes
            .SelectMany(x => x.EntityType
                .GetProperties()
                .Where(IsRichText)
                .Select(p => x.LocalizationType!.GetProperty(p.Name))
                .Where(p => p is not null && !IsRichText(p))
                .Select(p => $"{x.LocalizationType!.Name}.{p!.Name}"))
            .Distinct()
            .ToList();

        // Assert
        Assert.NotEmpty(translatedEntityTypes);
        Assert.Empty(unmarkedProperties);
    }

    private static Type? GetLocalizationType(Type entityType)
    {
        return entityType
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ITranslatedEntity<>))
            ?.GetGenericArguments()[0];
    }

    private static bool IsRichText(PropertyInfo property)
    {
        return property.IsDefined(typeof(RichTextAttribute), inherit: true);
    }
}
