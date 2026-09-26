using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.Create;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportFundsExpendituresRecords;

public class CreateReportFundsExpendituresRecordValidatorTests
{
    private readonly CreateReportFundsExpendituresRecordValidator _validator;
    private readonly int _currentYear;

    public CreateReportFundsExpendituresRecordValidatorTests()
    {
        var timeProvider = TimeProvider.System;
        _currentYear = timeProvider.GetUtcNow().Year;

        var recordDtoValidator = new CreateReportFundsExpendituresRecordDtoValidator(
            new BaseReportFundsExpendituresRecordValidator(),
            timeProvider);

        _validator = new CreateReportFundsExpendituresRecordValidator(recordDtoValidator);
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
    public void Validate_ShouldNotHaveErrors_WhenDataIsValid()
    {
        // Arrange
        var command = new CreateReportFundsExpendituresRecordCommand(GetValidDto());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveChildValidator_ForDto()
    {
        // Assert
        _validator.ShouldHaveChildValidator(
            x => x.CreateReportFundsExpendituresRecordDto,
            typeof(CreateReportFundsExpendituresRecordDtoValidator));
    }

    private CreateReportFundsExpendituresRecordDto GetValidDto() => new()
    {
        CategoryId = 1,
        Amount = 100.25m,
        Currency = ReportFundsExpendituresCurrency.Uah,
        Type = ReportFundsExpendituresType.Income,
        ReportingYear = _currentYear
    };
}
