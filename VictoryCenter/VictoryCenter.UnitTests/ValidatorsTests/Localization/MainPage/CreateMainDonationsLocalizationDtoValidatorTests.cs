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
    public void Validate_ShouldNotHaveErrors_WhenOptionalFieldsAreNull()
    {
        var dto = GetValidDto() with
        {
            Title = null,
            Description = null,
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
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
        var dto = GetValidDto() with { Title = new string('a', MainPageConstants.Title.MinLength - 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateMainDonationsLocalizationDto.Title), MainPageConstants.Title.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsTooLong()
    {
        var dto = GetValidDto() with { Title = new string('a', MainPageConstants.Title.MaxLength + 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateMainDonationsLocalizationDto.Title), MainPageConstants.Title.MaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooShort()
    {
        var dto = GetValidDto() with { Description = new string('a', MainPageConstants.Description.MinLength - 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateMainDonationsLocalizationDto.Description), MainPageConstants.Description.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooLong()
    {
        var dto = GetValidDto() with { Description = new string('a', MainPageConstants.Description.MaxLength + 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateMainDonationsLocalizationDto.Description), MainPageConstants.Description.MaxLength));
    }

    private static CreateMainDonationsLocalizationDto GetValidDto() => new()
    {
        EntityId = 1,
        Title = new string('a', MainPageConstants.Title.MinLength),
        Description = new string('a', MainPageConstants.Description.MinLength),
    };
}
