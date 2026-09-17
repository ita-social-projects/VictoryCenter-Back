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
        _validator = new CreateReportProgramExpendituresRecordCommandValidator();
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
    public void Validate_ShouldHaveError_WhenProgramCategoryIdIsNotPositive()
    {
        // Arrange
        var dto = GetValidDto() with { HippotherapyProgramCategoryId = 0 };
        var command = new CreateReportProgramExpendituresRecordCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x =>
                x.CreateReportProgramExpendituresRecordDto.HippotherapyProgramCategoryId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(ReportProgramExpendituresRecordDto.HippotherapyProgramCategoryId)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenReportingYearIsLessThanMin()
    {
        // Arrange
        var dto = GetValidDto() with
        {
            ReportingYear = ReportProgramExpendituresRecordConstants.ReportingYearMinValue - 1
        };
        var command = new CreateReportProgramExpendituresRecordCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreateReportProgramExpendituresRecordDto.ReportingYear)
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
        var command = new CreateReportProgramExpendituresRecordCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreateReportProgramExpendituresRecordDto.ReportingYear)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeLessThanOrEqualToN(
                nameof(ReportProgramExpendituresRecordDto.ReportingYear),
                ReportProgramExpendituresRecordConstants.ReportingYearMaxValue));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountUahIsNegative()
    {
        // Arrange
        var negativeAmount = -1.0m;
        var dto = GetValidDto() with { AmountUah = negativeAmount };
        var command = new CreateReportProgramExpendituresRecordCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreateReportProgramExpendituresRecordDto.AmountUah)
            .WithErrorMessage(ErrorMessagesConstants.SumMustNotBeNegative(
                nameof(ReportProgramExpendituresRecordDto.AmountUah)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountUahHasInvalidFormat()
    {
        // Arrange
        var dto = GetValidDto() with { AmountUah = 1.123m };
        var command = new CreateReportProgramExpendituresRecordCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreateReportProgramExpendituresRecordDto.AmountUah)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(
                nameof(ReportProgramExpendituresRecordDto.AmountUah),
                ReportProgramExpendituresRecordConstants.AmountFormat));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountUsdIsNegative()
    {
        // Arrange
        var negativeAmount = -1.00m;
        var dto = GetValidDto() with { AmountUsd = negativeAmount };
        var command = new CreateReportProgramExpendituresRecordCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreateReportProgramExpendituresRecordDto.AmountUsd)
            .WithErrorMessage(ErrorMessagesConstants.SumMustNotBeNegative(
                nameof(ReportProgramExpendituresRecordDto.AmountUsd)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountUsdHasInvalidFormat()
    {
        // Arrange
        var dto = GetValidDto() with { AmountUsd = 1.123m };
        var command = new CreateReportProgramExpendituresRecordCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreateReportProgramExpendituresRecordDto.AmountUsd)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(
                nameof(ReportProgramExpendituresRecordDto.AmountUsd),
                ReportProgramExpendituresRecordConstants.AmountFormat));
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

    [Fact]
    public void Validate_ShouldHoveError_WhenAmountUahIsZero()
    {
        // Arrange
        var zeroAmount = 0m;
        var dto = GetValidDto() with { AmountUah = zeroAmount };
        var command = new CreateReportProgramExpendituresRecordCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreateReportProgramExpendituresRecordDto.AmountUah)
            .WithErrorMessage(ErrorMessagesConstants.SumNotEqualTo(
                nameof(ReportProgramExpendituresRecordDto.AmountUah),
                zeroAmount));
    }

    [Fact]
    public void Validate_ShouldHoveError_WhenAmountUsdIsZero()
    {
        // Arrange
        var zeroAmount = 0m;
        var dto = GetValidDto() with { AmountUsd = zeroAmount };
        var command = new CreateReportProgramExpendituresRecordCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreateReportProgramExpendituresRecordDto.AmountUsd)
            .WithErrorMessage(ErrorMessagesConstants.SumNotEqualTo(
                nameof(ReportProgramExpendituresRecordDto.AmountUsd),
                zeroAmount));
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
