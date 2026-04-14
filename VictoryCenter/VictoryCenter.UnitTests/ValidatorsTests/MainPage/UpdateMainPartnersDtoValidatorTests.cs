using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.MainPartners;
using VictoryCenter.BLL.Validators.MainPage.Dto;

namespace VictoryCenter.UnitTests.ValidatorsTests.MainPage;

public class UpdateMainPartnersDtoValidatorTests
{
    private readonly UpdateMainPartnersDtoValidator _validator = new();

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDataIsValid()
    {
        var result = _validator.TestValidate(GetValidDto());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldHaveError_WhenTitleIsEmpty(string? title)
    {
        var dto = GetValidDto() with { Title = title! };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMainPartnersDto.Title)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsTooShort()
    {
        var dto = GetValidDto() with { Title = new string('a', MainPageConstants.Title.MinLength - 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainPartnersDto.Title), MainPageConstants.Title.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsTooLong()
    {
        var dto = GetValidDto() with { Title = new string('a', MainPageConstants.Title.MaxLength + 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainPartnersDto.Title), MainPageConstants.Title.MaxLength));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldHaveError_WhenDescriptionIsEmpty(string? description)
    {
        var dto = GetValidDto() with { Description = description! };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMainPartnersDto.Description)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooShort()
    {
        var dto = GetValidDto() with { Description = new string('a', MainPageConstants.Description.MinLength - 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseMainPartnersDto.Description), MainPageConstants.Description.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooLong()
    {
        var dto = GetValidDto() with { Description = new string('a', MainPageConstants.Description.MaxLength + 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMainPartnersDto.Description), MainPageConstants.Description.MaxLength));
    }

    private static UpdateMainPartnersDto GetValidDto() => new()
    {
        Title = "Partners title",
        Description = "Partners description",
    };
}