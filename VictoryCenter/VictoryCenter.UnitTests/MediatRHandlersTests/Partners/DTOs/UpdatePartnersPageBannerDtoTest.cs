using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Partners.DTOs;

public class UpdatePartnersPageBannerDtoTest
{
    [Theory]
    [InlineData("  Leading and trailing spaces", "Leading and trailing spaces")]
    [InlineData("Leading and trailing spaces  ", "Leading and trailing spaces")]
    [InlineData("  Leading and trailing spaces  ", "Leading and trailing spaces")]
    public void Description_ShouldTrimLeadingAndTrailingSpaces(string input, string expected)
    {
        // Arrange
        var dto = new UpdatePartnersPageBannerDto
        {
            Title = "Sample Title",
            Description = input,
            ImageId = 1
        };

        // Act
        var actualDescription = dto.Description;

        // Assert
        Assert.Equal(expected, actualDescription);
    }
}
