using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.Validators.MainPage.Dto;

namespace VictoryCenter.UnitTests.ValidatorsTests.MainPage;

public class CreateImpactStatisticDtoValidatorTests
{
    private readonly CreateImpactStatisticDtoValidator _validator = new();

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
    public void Validate_ShouldHaveError_WhenDescriptionIsEmpty(string? description)
    {
        var dto = GetValidDto() with { Description = description! };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseImpactStatisticDto.Description)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooShort()
    {
        var dto = GetValidDto() with
        {
            Description = new string('a', MainPageConstants.ImpactStatistic.Description.MinLength - 1),
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseImpactStatisticDto.Description),
                MainPageConstants.ImpactStatistic.Description.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooLong()
    {
        var dto = GetValidDto() with
        {
            Description = new string('a', MainPageConstants.ImpactStatistic.Description.MaxLength + 1),
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseImpactStatisticDto.Description),
                MainPageConstants.ImpactStatistic.Description.MaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMetricsIsNull()
    {
        var dto = GetValidDto() with { Metrics = null! };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Metrics)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateImpactStatisticDto.Metrics)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMetricsCountIsTooLarge()
    {
        var dto = GetValidDto() with
        {
            Metrics = Enumerable
                .Range(1, MainPageConstants.ImpactStatistic.MaxCount + 1)
                .Select(_ => new CreateMetricDto { Value = "100", Signature = "kids" })
                .ToList(),
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Metrics)
            .WithErrorMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(CreateImpactStatisticDto.Metrics), MainPageConstants.ImpactStatistic.MaxCount));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNestedMetricIsInvalid()
    {
        var dto = GetValidDto() with
        {
            Metrics = [new CreateMetricDto { Value = string.Empty, Signature = "kids" }],
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor("Metrics[0].Value");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMetricsContainsNullElement()
    {
        var dto = GetValidDto() with
        {
            Metrics = [null!],
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor("Metrics[0]");
    }

    private static CreateImpactStatisticDto GetValidDto() => new()
    {
        Description = "Impact description",
        ImageId = 1,
        Metrics = [new CreateMetricDto { Value = "100", Signature = "kids" }],
    };
}
