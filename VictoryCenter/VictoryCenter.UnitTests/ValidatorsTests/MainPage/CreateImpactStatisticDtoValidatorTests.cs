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
            Title = new string('a', MainPageConstants.ImpactStatistic.ValidationTitleRules.MinLen - 1),
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsTooLong()
    {
        var dto = GetValidDto() with
        {
            Title = new string('a', MainPageConstants.ImpactStatistic.ValidationTitleRules.MaxLen + 1),
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
            Metrics = [new CreateMetricDto { Value = 100, Name = "kids", Kind = MetricKind.Standard }],
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Metrics)
            .WithErrorMessage(ErrorMessagesConstants.MetricsMustContainExactlyNItems(MainPageConstants.ImpactStatistic.ExactMetricCount));
    }

    [Theory]
    [InlineData(MetricKind.Standard, MetricKind.Standard, MetricKind.Standard, MetricKind.Standard)]
    [InlineData(MetricKind.Standard, MetricKind.Standard, MetricKind.RaisedFunds, MetricKind.RaisedFunds)]
    [InlineData(MetricKind.RaisedFunds, MetricKind.RaisedFunds, MetricKind.RaisedFunds, MetricKind.RaisedFunds)]
    public void Validate_ShouldHaveError_WhenMetricKindCompositionIsInvalid(
        MetricKind first, MetricKind second, MetricKind third, MetricKind fourth)
    {
        var dto = GetValidDto() with
        {
            Metrics =
            [
                new CreateMetricDto { Value = 100, Name = "a", Kind = first },
                new CreateMetricDto { Value = 200, Name = "b", Kind = second },
                new CreateMetricDto { Value = 300, Name = "c", Kind = third },
                new CreateMetricDto { Value = 400, Name = "d", Kind = fourth },
            ],
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Metrics)
            .WithErrorMessage(ErrorMessagesConstants.MetricKindsMustMatchRequiredComposition(
                MainPageConstants.ImpactStatistic.ExactStandardMetricCount,
                MainPageConstants.ImpactStatistic.ExactRaisedFundsMetricCount));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenRaisedFundsMetricIsInAnyPosition()
    {
        var dto = GetValidDto() with
        {
            Metrics =
            [
                new CreateMetricDto { Value = 100, Name = "aa", Kind = MetricKind.RaisedFunds },
                new CreateMetricDto { Value = 200, Name = "bb", Kind = MetricKind.Standard },
                new CreateMetricDto { Value = 300, Name = "cc", Kind = MetricKind.Standard },
                new CreateMetricDto { Value = 400, Name = "dd", Kind = MetricKind.Standard },
            ],
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenLocalizationsSetOnAllMetrics()
    {
        var dto = GetValidDto() with
        {
            Metrics =
            [
                new CreateMetricDto
                {
                    Value = 100, Name = "aa", Kind = MetricKind.Standard,
                    Localization = new CreateMetricLocalizationDto { LanguageId = 1, Name = "Партнери" },
                },
                new CreateMetricDto
                {
                    Value = 200, Name = "bb", Kind = MetricKind.Standard,
                    Localization = new CreateMetricLocalizationDto { LanguageId = 1, Name = "Програми" },
                },
                new CreateMetricDto
                {
                    Value = 300, Name = "cc", Kind = MetricKind.RaisedFunds,
                    Localization = new CreateMetricLocalizationDto { LanguageId = 1, Name = "Зібрано" },
                },
                new CreateMetricDto
                {
                    Value = 400, Name = "dd", Kind = MetricKind.Standard,
                    Localization = new CreateMetricLocalizationDto { LanguageId = 1, Name = "Терапія" },
                },
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
                new CreateMetricDto { Value = -1, Name = "a", Kind = MetricKind.Standard },
                new CreateMetricDto { Value = 200, Name = "b", Kind = MetricKind.Standard },
                new CreateMetricDto { Value = 300, Name = "c", Kind = MetricKind.RaisedFunds },
                new CreateMetricDto { Value = 400, Name = "d", Kind = MetricKind.Standard },
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
            new CreateMetricDto { Value = 100, Name = "Partners", Kind = MetricKind.Standard },
            new CreateMetricDto { Value = 200, Name = "Programs", Kind = MetricKind.Standard },
            new CreateMetricDto { Value = 300, Name = "Raised", Kind = MetricKind.RaisedFunds },
            new CreateMetricDto { Value = 400, Name = "Therapy", Kind = MetricKind.Standard },
        ],
    };
}
