using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.Validators.MainPage.Dto;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.MainPage;

public class UpdateMetricDtoValidatorTests
{
    private readonly UpdateMetricDtoValidator _validator = new();

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDataIsValid()
    {
        var result = _validator.TestValidate(GetValidDto());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenIdIsNull()
    {
        var dto = GetValidDto() with { Id = null };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenIdIsNotPositive(long id)
    {
        var dto = GetValidDto() with { Id = id };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateMetricDto.Id)));
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
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldHaveError_WhenNameIsEmpty(string name)
    {
        var dto = GetValidDto() with { Name = name };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseMetricDto.Name)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameIsTooLong()
    {
        var dto = GetValidDto() with { Name = new string('a', MainPageConstants.Metric.Name.MaxLength + 1) };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseMetricDto.Name), MainPageConstants.Metric.Name.MaxLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenPrefixIsNull()
    {
        var dto = GetValidDto() with { Prefix = null };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.Prefix);
    }

    private static UpdateMetricDto GetValidDto() => new()
    {
        Id = 1,
        Value = 100,
        Name = "kids",
        Type = MetricType.Raised,
    };
}
