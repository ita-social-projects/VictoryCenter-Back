using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.ImpactStatistics.UpdateSingleMetric;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.MainPage;

public class UpdateSingleMetricCommandValidatorTests
{
    private readonly UpdateSingleMetricCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDataIsValid()
    {
        var command = new UpdateSingleMetricCommand(1, GetValidDto());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenMetricIdIsNotPositive(long id)
    {
        var command = new UpdateSingleMetricCommand(id, GetValidDto());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MetricId);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDtoIsNull()
    {
        var command = new UpdateSingleMetricCommand(1, null!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Dto);
    }

    private static UpdateSingleMetricDto GetValidDto() => new()
    {
        Value = 100,
        Name = "kids",
        Type = MetricType.Raised,
    };
}
