using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.BLL.Validators.Localization.MainPage;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.MainPage;

public class CreateMainDonationsLocalizationDtoValidatorTests
{
    private readonly CreateMainDonationsLocalizationDtoValidator _validator = new(new BaseMainPageLocalizationDtoValidator());

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDataIsValid()
    {
        var result = _validator.TestValidate(GetValidDto());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveErrors_WhenRequiredFieldsAreEmpty()
    {
        var dto = GetValidDto() with
        {
            Title = " ",
            Description = " ",
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateMainDonationsLocalizationDto.Title)));
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateMainDonationsLocalizationDto.Description)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenEntityIdIsNotPositive(long entityId)
    {
        var dto = GetValidDto() with { EntityId = entityId };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.EntityId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(CreateMainDonationsLocalizationDto.EntityId)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsTooShort()
    {
        var dto = GetValidDto() with { Title = new string('a', MainPageConstants.Localization.Title.MinLength - 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateMainDonationsLocalizationDto.Title), MainPageConstants.Localization.Title.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsTooLong()
    {
        var dto = GetValidDto() with { Title = new string('a', MainPageConstants.Localization.Title.MaxLength + 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateMainDonationsLocalizationDto.Title), MainPageConstants.Localization.Title.MaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooShort()
    {
        var dto = GetValidDto() with { Description = new string('a', MainPageConstants.Localization.SectionDescription.MinLength - 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateMainDonationsLocalizationDto.Description), MainPageConstants.Localization.SectionDescription.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooLong()
    {
        var dto = GetValidDto() with { Description = new string('a', MainPageConstants.Localization.SectionDescription.MaxLength + 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateMainDonationsLocalizationDto.Description), MainPageConstants.Localization.SectionDescription.MaxLength));
    }

    private static CreateMainDonationsLocalizationDto GetValidDto() => new()
    {
        EntityId = 1,
        Title = new string('a', MainPageConstants.Localization.Title.MinLength),
        Description = new string('a', MainPageConstants.Localization.SectionDescription.MinLength),
    };
}
