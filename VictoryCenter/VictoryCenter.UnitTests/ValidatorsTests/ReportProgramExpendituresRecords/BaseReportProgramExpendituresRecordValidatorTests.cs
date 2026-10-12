using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportProgramExpendituresRecords;

public class BaseReportProgramExpendituresRecordValidatorTests
{
    private readonly BaseReportProgramExpendituresRecordValidator _validator;

    public BaseReportProgramExpendituresRecordValidatorTests()
    {
        _validator = new BaseReportProgramExpendituresRecordValidator();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenProgramCategoryIdIsNotPositive()
    {
        // Arrange
        var dto = GetValidDto() with { HippotherapyProgramCategoryId = 0 };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x =>
                x.HippotherapyProgramCategoryId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(ReportProgramExpendituresRecordDto.HippotherapyProgramCategoryId)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountIsNegative()
    {
        // Arrange
        decimal negativeAmount = -1.5m;
        var dto = GetValidDto() with { AmountUah = negativeAmount };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AmountUah)
            .WithErrorMessage(ErrorMessagesConstants.SumMustNotBeNegative(
                nameof(BaseReportProgramExpendituresRecordDto.AmountUah)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountIsZero()
    {
        // Arrange
        decimal negativeAmount = 0;
        var dto = GetValidDto() with { AmountUah = negativeAmount };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AmountUah)
            .WithErrorMessage(ErrorMessagesConstants.SumNotEqualTo(
                nameof(BaseReportProgramExpendituresRecordDto.AmountUah),
                ReportProgramExpendituresRecordConstants.ZeroAmount));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountHasInvalidFormat()
    {
        // Arrange
        var dto = GetValidDto() with { AmountUah = 1.123m };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AmountUah)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(
                nameof(BaseReportProgramExpendituresRecordDto.AmountUah),
                ReportProgramExpendituresRecordConstants.AmountFormat));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountIsNull()
    {
        // Arrange
        var dto = GetValidDto() with { AmountUah = null };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AmountUah)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(BaseReportProgramExpendituresRecordDto.AmountUah)));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDataIsValid()
    {
        // Arrange
        var dto = GetValidDto();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static UpdateReportProgramExpendituresRecordDto GetValidDto() => new()
    {
        HippotherapyProgramCategoryId = 1,
        AmountUah = 100.25m,
        AmountUsd = 50.12m
    };
}
