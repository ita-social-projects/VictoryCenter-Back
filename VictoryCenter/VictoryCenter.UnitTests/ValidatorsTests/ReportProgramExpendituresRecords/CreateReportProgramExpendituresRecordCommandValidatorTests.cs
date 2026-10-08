using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportProgramExpendituresRecords;

public class CreateReportProgramExpendituresRecordCommandValidatorTests
{
    private readonly CreateReportProgramExpendituresRecordCommandValidator _validator;

    public CreateReportProgramExpendituresRecordCommandValidatorTests()
    {
        var timeProvider = TimeProvider.System;
        var recordDtoValidator = new CreateReportProgramExpendituresRecordDtoValidator(
            new BaseReportProgramExpendituresRecordValidator());

        _validator = new CreateReportProgramExpendituresRecordCommandValidator(recordDtoValidator);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDtoIsNull()
    {
        // Arrange
        var command = new CreateReportProgramExpendituresRecordCommand(null!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreateReportProgramExpendituresRecordDto);
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDataIsValid()
    {
        // Arrange
        var command = new CreateReportProgramExpendituresRecordCommand(GetValidDto());

        // Act
        var result = _validator.TestValidate(command);

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
