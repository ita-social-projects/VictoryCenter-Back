using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Partners.UpdateBanner;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Validators.Partners.Commands;

namespace VictoryCenter.UnitTests.ValidatorsTests.Partners;

public class UpdatePartnersPageBannerCommandValidatorTests
{
    private readonly UpdatePartnersPageBannerCommandValidator _validator = new();

    private readonly string _validTitle;
    private readonly string _validDescription;
    private readonly long _validImageId = 1;

    public UpdatePartnersPageBannerCommandValidatorTests()
    {
        _validTitle = new string('A', PartnerConstants.PartnersPageBannerTitleMinLength);
        _validDescription = new string('A', PartnerConstants.PartnersPageBannerDescriptionMinLength);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_TitleIsNullOrEmpty_ShouldHaveError(string title)
    {
        // Arrange
        // Use 'with' expression to create a new DTO with the invalid title
        var dto = GetValidDto() with { Title = title };
        var command = new UpdatePartnersPageBannerCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Title)
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdatePartnersPageBannerDto.Title)));
    }

    [Fact]
    public void Validate_TitleIsTooShort_ShouldHaveError()
    {
        // Arrange
        if (PartnerConstants.PartnersPageBannerTitleMinLength <= 1)
        {
            return;
        }

        var tooShortTitle = new string('A', PartnerConstants.PartnersPageBannerTitleMinLength - 1);

        // Use 'with' expression
        var dto = GetValidDto() with { Title = tooShortTitle };
        var command = new UpdatePartnersPageBannerCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Title)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                    nameof(UpdatePartnersPageBannerDto.Title), PartnerConstants.PartnersPageBannerTitleMinLength));
    }

    [Fact]
    public void Validate_TitleIsTooLong_ShouldHaveError()
    {
        // Arrange
        var tooLongTitle = new string('A', PartnerConstants.PartnersPageBannerTitleMaxLength + 1);

        // Use 'with' expression
        var dto = GetValidDto() with { Title = tooLongTitle };
        var command = new UpdatePartnersPageBannerCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Title)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(UpdatePartnersPageBannerDto.Title), PartnerConstants.PartnersPageBannerTitleMaxLength));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_DescriptionIsNullOrEmpty_ShouldHaveError(string description)
    {
        // Arrange
        // Use 'with' expression
        var dto = GetValidDto() with { Description = description };
        var command = new UpdatePartnersPageBannerCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Description)
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdatePartnersPageBannerDto.Description)));
    }

    [Fact]
    public void Validate_DescriptionIsTooShort_ShouldHaveError()
    {
        // Arrange
        if (PartnerConstants.PartnersPageBannerDescriptionMinLength <= 1)
        {
            return;
        }

        var tooShortDescription = new string('A', PartnerConstants.PartnersPageBannerDescriptionMinLength - 1);

        // Use 'with' expression
        var dto = GetValidDto() with { Description = tooShortDescription };
        var command = new UpdatePartnersPageBannerCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Description)
             .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                   nameof(UpdatePartnersPageBannerDto.Description), PartnerConstants.PartnersPageBannerDescriptionMinLength));
    }

    [Fact]
    public void Validate_DescriptionIsTooLong_ShouldHaveError()
    {
        // Arrange
        var tooLongDescription = new string('A', PartnerConstants.PartnersPageBannerDescriptionMaxLength + 1);

        // Use 'with' expression
        var dto = GetValidDto() with { Description = tooLongDescription };
        var command = new UpdatePartnersPageBannerCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Description)
             .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                   nameof(UpdatePartnersPageBannerDto.Description), PartnerConstants.PartnersPageBannerDescriptionMaxLength));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ImageIdIsNotPositive_ShouldHaveError(long imageId)
    {
        // Arrange
        // Use 'with' expression
        var dto = GetValidDto() with { ImageId = imageId };
        var command = new UpdatePartnersPageBannerCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.ImageId)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdatePartnersPageBannerDto.ImageId)));
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = GetValidDto();
        var command = new UpdatePartnersPageBannerCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    // Helper to create a valid DTO
    private UpdatePartnersPageBannerDto GetValidDto()
    {
        return new UpdatePartnersPageBannerDto
        {
            Title = _validTitle,
            Description = _validDescription,
            ImageId = _validImageId
        };
    }
}
