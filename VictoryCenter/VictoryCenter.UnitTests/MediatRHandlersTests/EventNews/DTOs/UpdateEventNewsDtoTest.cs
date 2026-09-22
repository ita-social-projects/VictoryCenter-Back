using VictoryCenter.BLL.DTOs.Admin.EventNews;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.EventNews.DTOs;

public class UpdateEventNewsDtoTest
{
    [Theory]
    [InlineData("Test Title ")]
    [InlineData(" Test Title ")]
    [InlineData(" Test     Title ")]
    public void UpdateEventNewsDto_ShouldNormalizeTitle(string title)
    {
        // Arrange, Act
        var expectedNormalizedTitle = "Test Title";
        var dto = new UpdateEventNewsDto
        {
            Title = title
        };

        // Assert
        Assert.Equal(expectedNormalizedTitle, dto.Title);
    }

    [Theory]
    [InlineData("Test Description ")]
    [InlineData(" Test Description ")]
    [InlineData(" Test     Description ")]
    public void UpdateEventNewsDto_ShouldNormalizeDescription(string description)
    {
        // Arrange, Act
        var expectedNormalizedDescription = "Test Description";
        var dto = new UpdateEventNewsDto
        {
            Description = description
        };

        // Assert
        Assert.Equal(expectedNormalizedDescription, dto.Description);
    }
}
