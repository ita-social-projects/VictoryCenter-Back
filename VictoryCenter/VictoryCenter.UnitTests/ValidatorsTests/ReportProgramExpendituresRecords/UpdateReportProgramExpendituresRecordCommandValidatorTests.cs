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
