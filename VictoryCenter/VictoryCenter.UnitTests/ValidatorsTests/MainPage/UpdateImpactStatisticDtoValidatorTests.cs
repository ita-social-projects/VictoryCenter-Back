using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.Validators.MainPage.Dto;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.MainPage;

public class UpdateImpactStatisticDtoValidatorTests
{
    private readonly UpdateImpactStatisticDtoValidator _validator = new();

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
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateImpactStatisticDto.Id)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMetricsContainDuplicateIds()
    {
        var dto = GetValidDto() with
        {
            Metrics =
            [
                new UpdateMetricDto { Id = 10, Value = 100, Name = "kids", Kind = MetricKind.RaisedFunds },
                new UpdateMetricDto { Id = 10, Value = 200, Name = "families", Kind = MetricKind.Standard },
                new UpdateMetricDto { Id = 11, Value = 300, Name = "programs", Kind = MetricKind.Standard },
                new UpdateMetricDto { Id = 12, Value = 400, Name = "hours", Kind = MetricKind.Standard },
            ],
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Metrics)
            .WithErrorMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(UpdateImpactStatisticDto.Metrics)));
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
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseImpactStatisticDto.Title)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsTooShort()
    {
        var dto = GetValidDto() with
        {
            Title = new string('a', MainPageConstants.ImpactStatistic.ValidationTitleRules.MinLen - 1),
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseImpactStatisticDto.Title),
                MainPageConstants.ImpactStatistic.ValidationTitleRules.MinLen));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsTooLong()
    {
        var dto = GetValidDto() with
        {
            Title = new string('a', MainPageConstants.ImpactStatistic.ValidationTitleRules.MaxLen + 1),
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseImpactStatisticDto.Title),
                MainPageConstants.ImpactStatistic.ValidationTitleRules.MaxLen));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMetricsIsNull()
    {
        var dto = GetValidDto() with { Metrics = null! };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Metrics)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateImpactStatisticDto.Metrics)));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Validate_ShouldHaveError_WhenMetricsCountIsNot4(int count)
    {
        var dto = GetValidDto() with
        {
            Metrics = Enumerable.Range(1, count)
                .Select(i => new UpdateMetricDto
                {
                    Id = i,
                    Value = 100,
                    Name = $"metric-{i}",
                    Kind = i == 1 ? MetricKind.RaisedFunds : MetricKind.Standard,
                })
                .ToList(),
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Metrics)
            .WithErrorMessage(ErrorMessagesConstants.MetricsMustContainExactlyNItems(MainPageConstants.ImpactStatistic.ExactMetricCount));
    }

    [Theory]
    [InlineData(MetricKind.Standard, MetricKind.Standard, MetricKind.Standard, MetricKind.Standard)]
    [InlineData(MetricKind.RaisedFunds, MetricKind.RaisedFunds, MetricKind.Standard, MetricKind.Standard)]
    public void Validate_ShouldHaveError_WhenMetricKindCompositionIsInvalid(
        MetricKind first, MetricKind second, MetricKind third, MetricKind fourth)
    {
        var dto = GetValidDto() with
        {
            Metrics =
            [
                new UpdateMetricDto { Id = 1, Value = 100, Name = "a", Kind = first },
                new UpdateMetricDto { Id = 2, Value = 200, Name = "b", Kind = second },
                new UpdateMetricDto { Id = 3, Value = 300, Name = "c", Kind = third },
                new UpdateMetricDto { Id = 4, Value = 400, Name = "d", Kind = fourth },
            ],
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Metrics)
            .WithErrorMessage(ErrorMessagesConstants.MetricKindsMustMatchRequiredComposition(
                MainPageConstants.ImpactStatistic.ExactStandardMetricCount,
                MainPageConstants.ImpactStatistic.ExactRaisedFundsMetricCount));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNestedMetricIsInvalid()
    {
        var dto = GetValidDto() with
        {
            Metrics =
            [
                new UpdateMetricDto { Id = 1, Value = -1, Name = "kids", Kind = MetricKind.RaisedFunds },
                new UpdateMetricDto { Id = 2, Value = 200, Name = "families", Kind = MetricKind.Standard },
                new UpdateMetricDto { Id = 3, Value = 300, Name = "programs", Kind = MetricKind.Standard },
                new UpdateMetricDto { Id = 4, Value = 400, Name = "hours", Kind = MetricKind.Standard },
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
            Metrics = [null!],
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor("Metrics[0]");
    }

    private static UpdateImpactStatisticDto GetValidDto() => new()
    {
        Id = 1,
        Title = "Impact title",
        ImageId = 1,
        Metrics =
        [
            new UpdateMetricDto { Id = 2, Value = 100, Name = "kids", Kind = MetricKind.RaisedFunds },
            new UpdateMetricDto { Id = 3, Value = 200, Name = "families", Kind = MetricKind.Standard },
            new UpdateMetricDto { Id = 4, Value = 300, Name = "programs", Kind = MetricKind.Standard },
            new UpdateMetricDto { Id = 5, Value = 400, Name = "hours", Kind = MetricKind.Standard },
        ],
    };
}
