using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportFundsExpendituresRecords;

public class CreateReportFundsExpendituresRecordDtoValidatorTests
{
    private readonly CreateReportFundsExpendituresRecordDtoValidator _validator;
    private readonly int _currentYear;

    public CreateReportFundsExpendituresRecordDtoValidatorTests()
    {
        var timeProvider = TimeProvider.System;
        _currentYear = timeProvider.GetUtcNow().Year;
        _validator = new CreateReportFundsExpendituresRecordDtoValidator(
            new BaseReportFundsExpendituresRecordValidator(),
            timeProvider);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenCategoryIdIsNotPositive()
    {
        // Arrange
        var dto = GetValidDto() with { CategoryId = 0 };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CategoryId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(ReportFundsExpendituresRecordDto.CategoryId)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTypeIsInvalid()
    {
        // Arrange
        var dto = GetValidDto() with { Type = (ReportFundsExpendituresType)99 };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Type)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeValidEnum(
                nameof(ReportFundsExpendituresRecordDto.Type)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenReportingYearIsLessThanMin()
    {
        // Arrange
        var dto = GetValidDto() with { ReportingYear = _currentYear - 2 };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ReportingYear)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeGreaterThanOrEqualToN(
                nameof(ReportFundsExpendituresRecordDto.ReportingYear),
                _currentYear - 1));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenReportingYearIsGreaterThanMax()
    {
        // Arrange
        var dto = GetValidDto() with { ReportingYear = _currentYear + 2 };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ReportingYear)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeLessThanOrEqualToN(
                nameof(ReportFundsExpendituresRecordDto.ReportingYear),
                _currentYear + 1));
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

    private CreateReportFundsExpendituresRecordDto GetValidDto() => new()
    {
        CategoryId = 1,
        Amount = 100.25m,
        Type = ReportFundsExpendituresType.Income,
        ReportingYear = _currentYear,
        Currency = ReportFundsExpendituresCurrency.Uah
    };
}
