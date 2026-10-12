using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportProgramExpendituresRecords;

public class CreateReportProgramExpendituresRecordDtoValidatorTests
{
    private readonly CreateReportProgramExpendituresRecordDtoValidator _validator;

    public CreateReportProgramExpendituresRecordDtoValidatorTests()
    {
        _validator = new CreateReportProgramExpendituresRecordDtoValidator(
            new BaseReportProgramExpendituresRecordValidator());
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenReportingYearIsLessThanMin()
    {
        // Arrange
        var dto = GetValidDto() with
        {
            ReportingYear = ReportProgramExpendituresRecordConstants.ReportingYearMinValue - 1
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ReportingYear)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeGreaterThanOrEqualToN(
                nameof(ReportProgramExpendituresRecordDto.ReportingYear),
                ReportProgramExpendituresRecordConstants.ReportingYearMinValue));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenReportingYearIsGreaterThanMax()
    {
        // Arrange
        var dto = GetValidDto() with
        {
            ReportingYear = ReportProgramExpendituresRecordConstants.ReportingYearMaxValue + 1
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ReportingYear)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeLessThanOrEqualToN(
                nameof(ReportProgramExpendituresRecordDto.ReportingYear),
                ReportProgramExpendituresRecordConstants.ReportingYearMaxValue));
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

    private static CreateReportProgramExpendituresRecordDto GetValidDto()
    {
        return new CreateReportProgramExpendituresRecordDto
        {
            HippotherapyProgramCategoryId = 1,
            AmountUah = 100.25m,
            AmountUsd = 50.50m,
            ReportingYear = ReportProgramExpendituresRecordConstants.ReportingYearMinValue
        };
    }
}
