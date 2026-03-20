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

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    public void Validate_ShouldHaveError_WhenAmountUahIsNotPositive(decimal amountUah)
    {
        // Arrange
        var dto = GetValidDto() with { AmountUah = amountUah };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AmountUah)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(
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

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    public void Validate_ShouldHaveError_WhenAmountUsdIsNotPositive(decimal amountUsd)
    {
        // Arrange
        var dto = GetValidDto() with { AmountUsd = amountUsd };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AmountUsd)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(
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

    private static UpdateReportFundsExpendituresRecordDto GetValidDto() => new()
    {
        CategoryId = 1,
        AmountUah = 100.25m,
        AmountUsd = 50.50m
    };
}
