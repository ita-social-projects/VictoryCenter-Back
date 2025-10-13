using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Partners.UpdateBanner;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Validators.Partners.Commands;

namespace VictoryCenter.UnitTests.ValidatorsTests.Partners;

public class UpdatePartnersPageBannerCommandValidatorTests
{
    private readonly UpdatePartnersPageBannerCommandValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_TitleIsNullOrEmpty_ShouldHaveError(string title)
    {
        // Arrange
        var command = new UpdatePartnersPageBannerCommand(new UpdatePartnersPageBannerDto { Title = title });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Title);
    }

    [Fact]
    public void Validate_TitleIsTooLong_ShouldHaveError()
    {
        // Arrange
        var tooLongTitle = new string('A', PartnerConstants.PartnersPageBannerTitleMaxLength + 1);
        var command = new UpdatePartnersPageBannerCommand(new UpdatePartnersPageBannerDto { Title = tooLongTitle });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Title);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_DescriptionIsNullOrEmpty_ShouldHaveError(string description)
    {
        // Arrange
        var command = new UpdatePartnersPageBannerCommand(new UpdatePartnersPageBannerDto { Description = description });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Description);
    }

    [Fact]
    public void Validate_DescriptionIsTooLong_ShouldHaveError()
    {
        // Arrange
        var tooLongDescription = new string('A', PartnerConstants.PartnersPageBannerDescriptionMaxLength + 1);
        var command = new UpdatePartnersPageBannerCommand(new UpdatePartnersPageBannerDto { Description = tooLongDescription });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Description);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ImageIdIsNotPositive_ShouldHaveError(long imageId)
    {
        // Arrange
        var command = new UpdatePartnersPageBannerCommand(new UpdatePartnersPageBannerDto { ImageId = imageId });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.ImageId);
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new UpdatePartnersPageBannerDto
        {
            Title = "Valid Title",
            Description = "Valid Description",
            ImageId = 1
        };
        var command = new UpdatePartnersPageBannerCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
