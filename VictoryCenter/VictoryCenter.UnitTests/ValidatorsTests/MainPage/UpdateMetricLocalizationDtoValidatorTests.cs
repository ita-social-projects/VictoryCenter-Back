using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage.Metrics;
using VictoryCenter.BLL.Validators.MainPage.Dto;

namespace VictoryCenter.UnitTests.ValidatorsTests.MainPage;

public class UpdateMetricLocalizationDtoValidatorTests
{
    private readonly UpdateMetricLocalizationDtoValidator _validator = new();

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDataIsValid()
    {
        var result = _validator.TestValidate(GetValidDto());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenPropertiesAreNull()
    {
        var dto = new UpdateMetricLocalizationDto();

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldHaveError_WhenNameIsEmpty(string name)
    {
        var dto = GetValidDto() with { Name = name };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateMetricLocalizationDto.Name)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameIsTooShort()
    {
        var dto = GetValidDto() with { Name = new string('a', MainPageConstants.Metric.ValidationNameRules.MinLen - 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateMetricLocalizationDto.Name), MainPageConstants.Metric.ValidationNameRules.MinLen));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameIsTooLong()
    {
        var dto = GetValidDto() with { Name = new string('a', MainPageConstants.Metric.ValidationNameRules.MaxLen + 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateMetricLocalizationDto.Name), MainPageConstants.Metric.ValidationNameRules.MaxLen));
    }

    private static UpdateMetricLocalizationDto GetValidDto() => new()
    {
        Name = "kids"
    };
}
