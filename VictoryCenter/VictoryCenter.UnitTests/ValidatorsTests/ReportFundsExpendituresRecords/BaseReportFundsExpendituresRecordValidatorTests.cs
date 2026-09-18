using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Enums;

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
    public void Validate_ShouldHaveError_WhenAmountIsNegative()
    {
        // Arrange
        var negativeAmount = -100m;
        var dto = GetValidDto() with { Amount = negativeAmount };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage(ErrorMessagesConstants.SumMustNotBeNegative(
                nameof(BaseReportFundsExpendituresRecordDto.Amount)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountHasInvalidFormat()
    {
        // Arrange
        var dto = GetValidDto() with { Amount = 1.123m };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(
                nameof(BaseReportFundsExpendituresRecordDto.Amount),
                ReportFundsExpendituresRecordConstants.AmountFormat));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountIsZero()
    {
        // Arrange
        var zeroAmount = 0m;
        var dto = GetValidDto() with { Amount = zeroAmount };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage(ErrorMessagesConstants.SumNotEqualTo(
                nameof(BaseReportFundsExpendituresRecordDto.Amount),
                ReportFundsExpendituresRecordConstants.ZeroAmount));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountIsNull()
    {
        // Arrange
        var dto = GetValidDto() with { Amount = null };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(BaseReportFundsExpendituresRecordDto.Amount)));
    }

    [Theory]
    [InlineData((ReportFundsExpendituresCurrency)0)]
    [InlineData((ReportFundsExpendituresCurrency)99)]
    public void Validate_ShouldHaveError_WhenCurrencyIsInvalid(ReportFundsExpendituresCurrency currency)
    {
        // Arrange
        var dto = GetValidDto() with { Currency = currency };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Currency)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeValidEnum(
                nameof(BaseReportFundsExpendituresRecordDto.Currency)));
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
        Amount = 100.25m,
        Currency = ReportFundsExpendituresCurrency.Uah
    };
}
