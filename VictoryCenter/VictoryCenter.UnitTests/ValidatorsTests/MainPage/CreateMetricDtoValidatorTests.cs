using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.Validators.MainPage.Dto;

namespace VictoryCenter.UnitTests.ValidatorsTests.MainPage;

public class CreateMetricDtoValidatorTests
{
    private readonly CreateMetricDtoValidator _validator = new();

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
    public void Validate_ShouldHaveError_WhenValueIsEmpty(string? value)
    {
        var dto = GetValidDto() with { Value = value! };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Value)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMetricDto.Value)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenValueIsTooLong()
    {
        var dto = GetValidDto() with { Value = new string('a', MainPageConstants.Metric.Value.MaxLength + 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Value)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMetricDto.Value), MainPageConstants.Metric.Value.MaxLength));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldHaveError_WhenSignatureIsEmpty(string? signature)
    {
        var dto = GetValidDto() with { Signature = signature! };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Signature)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMetricDto.Signature)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSignatureIsTooLong()
    {
        var dto = GetValidDto() with
        {
            Signature = new string('a', MainPageConstants.Metric.Signature.MaxLength + 1),
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Signature)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMetricDto.Signature), MainPageConstants.Metric.Signature.MaxLength));
    }

    private static CreateMetricDto GetValidDto() => new()
    {
        Value = "100",
        Signature = "children",
    };
}
