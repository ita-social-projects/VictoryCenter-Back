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
        var dto = GetValidDto() with { Title = new string('a', MainPageConstants.Localization.ValidationTitleRules.MinLen - 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateMainDonationsLocalizationDto.Title), MainPageConstants.Localization.ValidationTitleRules.MinLen));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsTooLong()
    {
        var dto = GetValidDto() with { Title = new string('a', MainPageConstants.Localization.ValidationTitleRules.MaxLen + 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateMainDonationsLocalizationDto.Title), MainPageConstants.Localization.ValidationTitleRules.MaxLen));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooShort()
    {
        var dto = GetValidDto() with { Description = new string('a', MainPageConstants.Localization.ValidationSectionDescriptionRules.MinLen - 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateMainDonationsLocalizationDto.Description), MainPageConstants.Localization.ValidationSectionDescriptionRules.MinLen));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooLong()
    {
        var dto = GetValidDto() with { Description = new string('a', MainPageConstants.Localization.ValidationSectionDescriptionRules.MaxLen + 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateMainDonationsLocalizationDto.Description), MainPageConstants.Localization.ValidationSectionDescriptionRules.MaxLen));
    }

    private static CreateMainDonationsLocalizationDto GetValidDto() => new()
    {
        EntityId = 1,
        Title = new string('a', MainPageConstants.Localization.ValidationTitleRules.MinLen),
        Description = new string('a', MainPageConstants.Localization.ValidationSectionDescriptionRules.MinLen),
    };
}
