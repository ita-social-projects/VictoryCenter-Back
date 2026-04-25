using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage.Metrics;
using VictoryCenter.BLL.Validators.MainPage.Dto;
using VictoryCenter.DAL.Enums;

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
    public void Validate_ShouldHaveError_WhenTitleIsEmpty(string? title)
    {
        var dto = GetValidDto() with { Title = title! };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsTooShort()
    {
        var dto = GetValidDto() with
        {
            Title = new string('a', MainPageConstants.ImpactStatistic.Title.MinLength - 1),
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsTooLong()
    {
        var dto = GetValidDto() with
        {
            Title = new string('a', MainPageConstants.ImpactStatistic.Title.MaxLength + 1),
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMetricsIsNull()
    {
        var dto = GetValidDto() with { Metrics = null! };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Metrics);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMetricsCountIsNot4()
    {
        var dto = GetValidDto() with
        {
            Metrics = [new CreateMetricDto { Value = 100, Name = "kids", Type = MetricType.Partners }],
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Metrics)
            .WithErrorMessage($"Metrics must contain exactly {MainPageConstants.ImpactStatistic.ExactMetricCount} items.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMetricTypesAreNotUnique()
    {
        var dto = GetValidDto() with
        {
            Metrics =
            [
                new CreateMetricDto { Value = 100, Name = "a", Type = MetricType.Partners },
                new CreateMetricDto { Value = 200, Name = "b", Type = MetricType.Partners },
                new CreateMetricDto { Value = 300, Name = "c", Type = MetricType.Programs },
                new CreateMetricDto { Value = 400, Name = "d", Type = MetricType.TherapyHours },
            ],
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Metrics);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenLocalizationSetOnNonRaicedMetric()
    {
        var dto = GetValidDto() with
        {
            Metrics =
            [
                new CreateMetricDto
                {
                    Value = 100, Name = "a", Type = MetricType.Partners,
                    Localization = new CreateMetricLocalizationDto { LanguageId = 1, Name = "Partners" },
                },
                new CreateMetricDto { Value = 200, Name = "b", Type = MetricType.Programs },
                new CreateMetricDto { Value = 300, Name = "c", Type = MetricType.Raiced },
                new CreateMetricDto { Value = 400, Name = "d", Type = MetricType.TherapyHours },
            ],
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrors();
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenLocalizationSetOnRaicedMetric()
    {
        var dto = GetValidDto() with
        {
            Metrics =
            [
                new CreateMetricDto { Value = 100, Name = "a", Type = MetricType.Partners },
                new CreateMetricDto { Value = 200, Name = "b", Type = MetricType.Programs },
                new CreateMetricDto
                {
                    Value = 300, Name = "c", Type = MetricType.Raiced,
                    Localization = new CreateMetricLocalizationDto { LanguageId = 1, Name = "Зібрано" },
                },
                new CreateMetricDto { Value = 400, Name = "d", Type = MetricType.TherapyHours },
            ],
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNestedMetricIsInvalid()
    {
        var dto = GetValidDto() with
        {
            Metrics =
            [
                new CreateMetricDto { Value = -1, Name = "a", Type = MetricType.Partners },
                new CreateMetricDto { Value = 200, Name = "b", Type = MetricType.Programs },
                new CreateMetricDto { Value = 300, Name = "c", Type = MetricType.Raiced },
                new CreateMetricDto { Value = 400, Name = "d", Type = MetricType.TherapyHours },
            ],
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor("Metrics[0].Value");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMetricsContainsNullElement()
    {
        var dto = GetValidDto() with
        {
            Metrics = [null!, null!, null!, null!],
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor("Metrics[0]");
    }

    private static CreateImpactStatisticDto GetValidDto() => new()
    {
        Title = "Impact title",
        ImageId = 1,
        Metrics =
        [
            new CreateMetricDto { Value = 100, Name = "Partners", Type = MetricType.Partners },
            new CreateMetricDto { Value = 200, Name = "Programs", Type = MetricType.Programs },
            new CreateMetricDto { Value = 300, Name = "Raised", Type = MetricType.Raiced },
            new CreateMetricDto { Value = 400, Name = "Therapy", Type = MetricType.TherapyHours },
        ],
    };
}
