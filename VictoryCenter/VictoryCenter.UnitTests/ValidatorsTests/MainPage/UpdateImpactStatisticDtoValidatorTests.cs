using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.Validators.MainPage.Dto;

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
                new UpdateMetricDto { Id = 10, Value = "100", Signature = "kids" },
                new UpdateMetricDto { Id = 10, Value = "200", Signature = "families" },
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
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateImpactStatisticDto.Metrics)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMetricsCountIsTooLarge()
    {
        var dto = GetValidDto() with
        {
            Metrics = Enumerable
                .Range(1, MainPageConstants.ImpactStatistic.MaxCount + 1)
                .Select(i => new UpdateMetricDto { Id = i, Value = "100", Signature = "kids" })
                .ToList(),
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Metrics)
            .WithErrorMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(UpdateImpactStatisticDto.Metrics), MainPageConstants.ImpactStatistic.MaxCount));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNestedMetricIsInvalid()
    {
        var dto = GetValidDto() with
        {
            Metrics = [new UpdateMetricDto { Id = 1, Value = string.Empty, Signature = "kids" }],
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
        Description = "Impact description",
        ImageId = 1,
        Metrics = [new UpdateMetricDto { Id = 2, Value = "100", Signature = "kids" }],
    };
}