using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportFundsExpendituresRecords;

public class BaseReportFundsExpendituresRecordValidatorTests
{
    private readonly BaseReportFundsExpendituresRecordValidator _validator;

    public BaseReportFundsExpendituresRecordValidatorTests()
    {
        _validator = new BaseReportFundsExpendituresRecordValidator();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenCategoryIdIsNotPositive(long categoryId)
    {
        // Arrange
        var dto = GetValidDto() with { CategoryId = categoryId };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CategoryId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(ReportFundsExpendituresRecordDto.CategoryId)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountUahIsNegative()
    {
        // Arrange
        var negativeAmountUah = -100m;
        var dto = GetValidDto() with { AmountUah = negativeAmountUah };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AmountUah)
            .WithErrorMessage(ErrorMessagesConstants.SumMustNotBeNegative(
                nameof(ReportFundsExpendituresRecordDto.AmountUah)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountUahHasInvalidFormat()
    {
        // Arrange
        var dto = GetValidDto() with { AmountUah = 1.123m };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AmountUah)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(
                nameof(ReportFundsExpendituresRecordDto.AmountUah),
                ReportFundsExpendituresRecordConstants.AmountFormat));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountUsdIsNegative()
    {
        // Arrange
        var negativeAmountUsd = -100m;
        var dto = GetValidDto() with { AmountUsd = negativeAmountUsd };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AmountUsd)
            .WithErrorMessage(ErrorMessagesConstants.SumMustNotBeNegative(
                nameof(ReportFundsExpendituresRecordDto.AmountUsd)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountUsdHasInvalidFormat()
    {
        // Arrange
        var dto = GetValidDto() with { AmountUsd = 1.123m };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AmountUsd)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(
                nameof(ReportFundsExpendituresRecordDto.AmountUsd),
                ReportFundsExpendituresRecordConstants.AmountFormat));
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

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountUahIsZero()
    {
        // Arrange
        var zeroAmountUah = 0m;
        var dto = GetValidDto() with { AmountUah = zeroAmountUah };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AmountUah)
            .WithErrorMessage(ErrorMessagesConstants.SumNotEqualTo(
                nameof(ReportFundsExpendituresRecordDto.AmountUah),
                ReportFundsExpendituresRecordConstants.ZeroAmount));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountUsdIsZero()
    {
        // Arrange
        var zeroAmountUsd = 0m;
        var dto = GetValidDto() with { AmountUsd = zeroAmountUsd };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AmountUsd)
            .WithErrorMessage(ErrorMessagesConstants.SumNotEqualTo(
                nameof(ReportFundsExpendituresRecordDto.AmountUsd),
                ReportFundsExpendituresRecordConstants.ZeroAmount));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountUahIsNull()
    {
        // Arrange
        var nullAmountUah = (decimal?)null;
        var dto = GetValidDto() with { AmountUah = nullAmountUah };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AmountUah)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(ReportFundsExpendituresRecordDto.AmountUah)));
    }

    [Fact]
    public async Task Validate_ShouldHaveError_WhenAmountUsdIsNull()
    {
        // Arrange
        var nullAmountUsd = (decimal?)null;
        var dto = GetValidDto() with { AmountUsd = nullAmountUsd };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AmountUsd)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(ReportFundsExpendituresRecordDto.AmountUsd)));
    }

    private static UpdateReportFundsExpendituresRecordDto GetValidDto() => new()
    {
        CategoryId = 1,
        AmountUah = 100.25m,
        AmountUsd = 50.50m
    };
}
