using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.Validators.MainPage.Dto;
using VictoryCenter.DAL.Enums;

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

    [Fact]
    public void Validate_ShouldHaveError_WhenValueIsNegative()
    {
        var dto = GetValidDto() with { Value = -1 };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Value)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMetricDto.Value)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldHaveError_WhenNameIsEmpty(string? signature)
    {
        var dto = GetValidDto() with { Name = signature! };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMetricDto.Name)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameIsTooLong()
    {
        var dto = GetValidDto() with
        {
            Name = new string('a', MainPageConstants.Metric.ValidationNameRules.MaxLen + 1),
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMetricDto.Name), MainPageConstants.Metric.ValidationNameRules.MaxLen));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTypeIsInvalid()
    {
        var dto = GetValidDto() with { Type = (MetricType)999 };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPrefixIsInvalid()
    {
        var dto = GetValidDto() with { Prefix = (MetricPrefix)999 };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Prefix);
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenPrefixIsNull()
    {
        var dto = GetValidDto() with { Prefix = null };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.Prefix);
    }

    private static CreateMetricDto GetValidDto() => new()
    {
        Value = 100,
        Name = "children",
        Type = MetricType.Partners,
    };
}
