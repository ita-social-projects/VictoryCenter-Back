using FluentValidation.TestHelper;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportProgramExpendituresRecords;

public class UpdateReportProgramExpendituresRecordCommandValidatorTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly UpdateReportProgramExpendituresRecordCommandValidator _validator;

    public UpdateReportProgramExpendituresRecordCommandValidatorTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();

        _repositoryWrapperMock
            .Setup(x => x.HippotherapyProgramCategoriesRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<HippotherapyProgramCategory>>()))
            .ReturnsAsync(new HippotherapyProgramCategory());

        _validator = new UpdateReportProgramExpendituresRecordCommandValidator(_repositoryWrapperMock.Object);
    }

    [Fact]
    public async Task Validate_ShouldHaveError_WhenRecordIdIsNotPositive()
    {
        // Arrange
        var command = new UpdateReportProgramExpendituresRecordCommand(0, GetValidDto());

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ReportProgramExpendituresRecordId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(UpdateReportProgramExpendituresRecordCommand.ReportProgramExpendituresRecordId)));
    }

    [Fact]
    public async Task Validate_ShouldHaveError_WhenProgramCategoryIdIsNotPositive()
    {
        // Arrange
        var dto = GetValidDto() with { HippotherapyProgramCategoryId = 0 };
        var command = new UpdateReportProgramExpendituresRecordCommand(1, dto);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.HippotherapyProgramCategoryId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(UpdateReportProgramExpendituresRecordDto.HippotherapyProgramCategoryId)));
    }

    [Fact]
    public async Task Validate_ShouldHaveError_WhenProgramCategoryDoesNotExist()
    {
        // Arrange
        _repositoryWrapperMock
            .Setup(x => x.HippotherapyProgramCategoriesRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<HippotherapyProgramCategory>>()))
            .ReturnsAsync((HippotherapyProgramCategory)null!);

        var command = new UpdateReportProgramExpendituresRecordCommand(1, GetValidDto());

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.HippotherapyProgramCategoryId)
            .WithErrorMessage(ErrorMessagesConstants.NotFound());
    }

    [Fact]
    public async Task Validate_ShouldNotHaveErrors_WhenDataIsValid()
    {
        // Arrange
        var command = new UpdateReportProgramExpendituresRecordCommand(1, GetValidDto());

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_ShouldHaveErrors_WhenAmountUahIsNegative()
    {
        // Arrange
        var negativeAmount = -100.25m;
        var dto = GetValidDto() with { AmountUah = negativeAmount };
        var command = new UpdateReportProgramExpendituresRecordCommand(1, dto);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.AmountUah)
            .WithErrorMessage(ErrorMessagesConstants.SumMustNotBeNegative(
                nameof(UpdateReportProgramExpendituresRecordDto.AmountUah)));
    }

    [Fact]
    public async Task Validate_ShouldHaveErrors_WhenAmountUsdIsNegative()
    {
        // Arrange
        var negativeAmount = -50.50m;
        var dto = GetValidDto() with { AmountUsd = negativeAmount };
        var command = new UpdateReportProgramExpendituresRecordCommand(1, dto);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.AmountUsd)
            .WithErrorMessage(ErrorMessagesConstants.SumMustNotBeNegative(
                nameof(UpdateReportProgramExpendituresRecordDto.AmountUsd)));
    }

    [Fact]
    public async Task Validate_ShouldHaveErrors_WhenAmountUahIsZero()
    {
        // Arrange
        var zeroAmount = 0m;
        var dto = GetValidDto() with { AmountUah = zeroAmount };
        var command = new UpdateReportProgramExpendituresRecordCommand(1, dto);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.AmountUah)
            .WithErrorMessage(ErrorMessagesConstants.SumNotEqualTo(
                nameof(UpdateReportProgramExpendituresRecordDto.AmountUah),
                zeroAmount));
    }

    [Fact]
    public async Task Validate_ShouldHaveErrors_WhenAmountUsdIsZero()
    {
        // Arrange
        var zeroAmount = 0m;
        var dto = GetValidDto() with { AmountUsd = zeroAmount };
        var command = new UpdateReportProgramExpendituresRecordCommand(1, dto);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.AmountUsd)
            .WithErrorMessage(ErrorMessagesConstants.SumNotEqualTo(
                nameof(UpdateReportProgramExpendituresRecordDto.AmountUsd),
                zeroAmount));
    }

    private static UpdateReportProgramExpendituresRecordDto GetValidDto()
    {
        return new UpdateReportProgramExpendituresRecordDto
        {
            HippotherapyProgramCategoryId = 1,
            AmountUah = 100.25m,
            AmountUsd = 50.50m
        };
    }
}
