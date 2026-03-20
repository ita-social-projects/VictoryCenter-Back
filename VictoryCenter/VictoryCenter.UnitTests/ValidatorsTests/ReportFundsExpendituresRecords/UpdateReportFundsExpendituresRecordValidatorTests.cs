using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportFundsExpendituresRecords;

public class UpdateReportFundsExpendituresRecordValidatorTests
{
    private readonly UpdateReportFundsExpendituresRecordValidator _validator;

    public UpdateReportFundsExpendituresRecordValidatorTests()
    {
        _validator = new UpdateReportFundsExpendituresRecordValidator(
            new BaseReportFundsExpendituresRecordValidator());
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDtoIsNull()
    {
        // Arrange
        var command = new UpdateReportFundsExpendituresRecordCommand(null!, 1);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateReportFundsExpendituresRecordDto);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenCategoryIdIsNotPositive()
    {
        // Arrange
        var dto = GetValidDto() with { CategoryId = 0 };
        var command = new UpdateReportFundsExpendituresRecordCommand(dto, 1);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateReportFundsExpendituresRecordDto.CategoryId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(ReportFundsExpendituresRecordDto.CategoryId)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountUahHasInvalidFormat()
    {
        // Arrange
        var dto = GetValidDto() with { AmountUah = 1.123m };
        var command = new UpdateReportFundsExpendituresRecordCommand(dto, 1);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateReportFundsExpendituresRecordDto.AmountUah)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(
                nameof(ReportFundsExpendituresRecordDto.AmountUah),
                ReportFundsExpendituresRecordConstants.AmountFormat));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDataIsValid()
    {
        // Arrange
        var command = new UpdateReportFundsExpendituresRecordCommand(GetValidDto(), 1);

        // Act
        var result = _validator.TestValidate(command);

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
