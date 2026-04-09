using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.ReportProgramExpendituresCategories;
using VictoryCenter.DAL.Repositories.Interfaces.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportProgramExpendituresRecords;

public class CreateReportProgramExpendituresRecordTests
{
    private readonly Mock<IReportProgramExpendituresCategoriesRepository> _categoriesRepositoryMock;

    private readonly ReportProgramExpendituresCategory _category = new()
    {
        Id = 1,
        Name = "Program category"
    };

    private readonly CreateReportProgramExpendituresRecordDto _createDto = new()
    {
        ProgramCategoryId = 1,
        ReportingYear = ReportProgramExpendituresRecordConstants.ReportingYearMinValue,
        AmountUah = 100.50m,
        AmountUsd = 25.25m
    };

    private readonly Mock<IMapper> _mapperMock;

    private readonly ReportProgramExpendituresRecordDto _recordDto = new()
    {
        Id = 1,
        ProgramCategoryId = 1,
        ReportingYear = ReportProgramExpendituresRecordConstants.ReportingYearMinValue,
        AmountUah = 100.50m,
        AmountUsd = 25.25m
    };

    private readonly ReportProgramExpendituresRecord _recordEntity = new()
    {
        Id = 1,
        ProgramCategoryId = 1,
        ReportingYear = ReportProgramExpendituresRecordConstants.ReportingYearMinValue,
        AmountUah = 100.50m,
        AmountUsd = 25.25m
    };

    private readonly Mock<IReportProgramExpendituresRecordsRepository> _recordsRepositoryMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IValidator<CreateReportProgramExpendituresRecordCommand> _validator;

    public CreateReportProgramExpendituresRecordTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _recordsRepositoryMock = new Mock<IReportProgramExpendituresRecordsRepository>();
        _categoriesRepositoryMock = new Mock<IReportProgramExpendituresCategoriesRepository>();
        _validator = new CreateReportProgramExpendituresRecordCommandValidator();
    }

    [Fact]
    public async Task Handle_ShouldCreateRecord()
    {
        // Arrange
        SetupDependencies(_category, 1);
        var handler = new CreateReportProgramExpendituresRecordHandler(
            _validator,
            _repositoryWrapperMock.Object,
            _mapperMock.Object);

        // Act
        var result = await handler.Handle(
            new CreateReportProgramExpendituresRecordCommand(_createDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_recordDto.ProgramCategoryId, result.Value.ProgramCategoryId);
        Assert.Equal(_recordDto.AmountUah, result.Value.AmountUah);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var invalidDto = _createDto with { ProgramCategoryId = 0 };
        var handler = new CreateReportProgramExpendituresRecordHandler(
            _validator,
            _repositoryWrapperMock.Object,
            _mapperMock.Object);

        // Act
        var result = await handler.Handle(
            new CreateReportProgramExpendituresRecordCommand(invalidDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Validation failed", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCategoryNotFound()
    {
        // Arrange
        SetupDependencies(null, 1);
        var handler = new CreateReportProgramExpendituresRecordHandler(
            _validator,
            _repositoryWrapperMock.Object,
            _mapperMock.Object);

        // Act
        var result = await handler.Handle(
            new CreateReportProgramExpendituresRecordCommand(_createDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(_createDto.ProgramCategoryId, typeof(ReportProgramExpendituresCategory)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCategoryAlreadyHasRecord()
    {
        // Arrange
        SetupDependencies(_category, 1, _recordEntity);
        var handler = new CreateReportProgramExpendituresRecordHandler(
            _validator,
            _repositoryWrapperMock.Object,
            _mapperMock.Object);

        // Act
        var result = await handler.Handle(
            new CreateReportProgramExpendituresRecordCommand(_createDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ReportProgramExpendituresRecordConstants.ProgramCategoryAlreadyHasRecord,
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        // Arrange
        SetupDependencies(_category, 0);
        var handler = new CreateReportProgramExpendituresRecordHandler(
            _validator,
            _repositoryWrapperMock.Object,
            _mapperMock.Object);

        // Act
        var result = await handler.Handle(
            new CreateReportProgramExpendituresRecordCommand(_createDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(ReportProgramExpendituresRecord)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionOccurs()
    {
        // Arrange
        SetupDependencies(_category, 1);
        _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());

        var handler = new CreateReportProgramExpendituresRecordHandler(
            _validator,
            _repositoryWrapperMock.Object,
            _mapperMock.Object);

        // Act
        var result = await handler.Handle(
            new CreateReportProgramExpendituresRecordCommand(_createDto),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(ReportProgramExpendituresRecord)),
            result.Errors[0].Message);
    }

    private void SetupDependencies(
        ReportProgramExpendituresCategory? category,
        int saveResult,
        ReportProgramExpendituresRecord? existingRecordInCategory = null)
    {
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportProgramExpendituresRecordsRepository)
            .Returns(_recordsRepositoryMock.Object);
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportProgramExpendituresCategoriesRepository)
            .Returns(_categoriesRepositoryMock.Object);

        _categoriesRepositoryMock
            .Setup(repository =>
                repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ReportProgramExpendituresCategory>>()))
            .ReturnsAsync(category);

        _recordsRepositoryMock
            .Setup(repository => repository.CreateAsync(It.IsAny<ReportProgramExpendituresRecord>()))
            .ReturnsAsync((ReportProgramExpendituresRecord record) => record);

        _recordsRepositoryMock
            .Setup(repository =>
                repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ReportProgramExpendituresRecord>>()))
            .ReturnsAsync(existingRecordInCategory);

        _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(saveResult);

        _mapperMock
            .Setup(mapper =>
                mapper.Map<ReportProgramExpendituresRecord>(It.IsAny<CreateReportProgramExpendituresRecordDto>()))
            .Returns(_recordEntity);
        _mapperMock
            .Setup(mapper =>
                mapper.Map<ReportProgramExpendituresRecordDto>(It.IsAny<ReportProgramExpendituresRecord>()))
            .Returns(_recordDto);
    }
}
