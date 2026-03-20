using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportFundsExpendituresRecords;

public class CreateReportFundsExpendituresRecordValidatorTests
{
    private readonly CreateReportFundsExpendituresRecordValidator _validator;

    public CreateReportFundsExpendituresRecordValidatorTests()
    {
        _validator = new CreateReportFundsExpendituresRecordValidator(
            new BaseReportFundsExpendituresRecordValidator());
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDtoIsNull()
    {
        // Arrange
        var command = new CreateReportFundsExpendituresRecordCommand(null!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreateReportFundsExpendituresRecordDto);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenCategoryIdIsNotPositive()
    {
        // Arrange
        var dto = GetValidDto() with { CategoryId = 0 };
        var command = new CreateReportFundsExpendituresRecordCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreateReportFundsExpendituresRecordDto.CategoryId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(ReportFundsExpendituresRecordDto.CategoryId)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTypeIsInvalid()
    {
        // Arrange
        var dto = GetValidDto() with { Type = (ReportFundsExpendituresType)99 };
        var command = new CreateReportFundsExpendituresRecordCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreateReportFundsExpendituresRecordDto.Type)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeValidEnum(
                nameof(ReportFundsExpendituresRecordDto.Type)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenReportingYearIsLessThanMin()
    {
        // Arrange
        var dto = GetValidDto() with
        {
            ReportingYear = ReportFundsExpendituresRecordConstants.ReportingYearMinValue - 1
        };
        var command = new CreateReportFundsExpendituresRecordCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreateReportFundsExpendituresRecordDto.ReportingYear)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeGreaterThanOrEqualToN(
                nameof(ReportFundsExpendituresRecordDto.ReportingYear),
                ReportFundsExpendituresRecordConstants.ReportingYearMinValue));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenReportingYearIsGreaterThanMax()
    {
        // Arrange
        var dto = GetValidDto() with
        {
            ReportingYear = ReportFundsExpendituresRecordConstants.ReportingYearMaxValue + 1
        };
        var command = new CreateReportFundsExpendituresRecordCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreateReportFundsExpendituresRecordDto.ReportingYear)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeLessThanOrEqualToN(
                nameof(ReportFundsExpendituresRecordDto.ReportingYear),
                ReportFundsExpendituresRecordConstants.ReportingYearMaxValue));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDataIsValid()
    {
        // Arrange
        var command = new CreateReportFundsExpendituresRecordCommand(GetValidDto());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateReportFundsExpendituresRecordDto GetValidDto() => new()
    {
        CategoryId = 1,
        AmountUah = 100.25m,
        AmountUsd = 50.50m,
        Type = ReportFundsExpendituresType.Income,
        ReportingYear = ReportFundsExpendituresRecordConstants.ReportingYearMinValue
    };
}
