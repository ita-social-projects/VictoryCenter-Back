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
using VictoryCenter.DAL.Repositories.Interfaces.HippotherapyProgramCategories;
using VictoryCenter.DAL.Repositories.Interfaces.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportProgramExpendituresRecords;

public class CreateReportProgramExpendituresRecordTests
{
    private readonly HippotherapyProgramCategory _category = new()
    {
        Id = 1,
        Name = "Program category"
    };

    private readonly CreateReportProgramExpendituresRecordDto _createDto = new()
    {
        HippotherapyProgramCategoryId = 1,
        ReportingYear = ReportProgramExpendituresRecordConstants.ReportingYearMinValue,
        AmountUah = 100.50m,
        AmountUsd = 25.25m
    };

    private readonly Mock<IHippotherapyProgramCategoriesRepository> _hippotherapyProgramCategoriesRepositoryMock;

    private readonly Mock<IMapper> _mapperMock;

    private readonly ReportProgramExpendituresRecordDto _recordDto = new()
    {
        Id = 1,
        HippotherapyProgramCategoryId = 1,
        ReportingYear = ReportProgramExpendituresRecordConstants.ReportingYearMinValue,
        AmountUah = 100.50m,
        AmountUsd = 25.25m
    };

    private readonly ReportProgramExpendituresRecord _recordEntity = new()
    {
        Id = 1,
        HippotherapyProgramCategoryId = 1,
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
        _hippotherapyProgramCategoriesRepositoryMock = new Mock<IHippotherapyProgramCategoriesRepository>();
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
        Assert.Equal(_recordDto.HippotherapyProgramCategoryId, result.Value.HippotherapyProgramCategoryId);
        Assert.Equal(_recordDto.AmountUah, result.Value.AmountUah);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var invalidDto = _createDto with { HippotherapyProgramCategoryId = 0 };
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
        Assert.Equal(
            ErrorMessagesConstants.PropertyMustBePositive(
                nameof(ReportProgramExpendituresRecord.HippotherapyProgramCategoryId)),
            result.Errors[0].Message);
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
            ErrorMessagesConstants.NotFound(
                _createDto.HippotherapyProgramCategoryId,
                typeof(HippotherapyProgramCategory)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCategoryAlreadyHasRecordForSpecifiedYear()
    {
        // Arrange
        SetupDependencies(_category, 1, true);
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
            ReportProgramExpendituresRecordConstants.ProgramCategoryAlreadyHasRecordForSpecifiedYear(
                _createDto.HippotherapyProgramCategoryId, _createDto.ReportingYear),
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
        HippotherapyProgramCategory? category,
        int saveResult,
        bool recordWithinSameCategoryWithSameYearExists = false)
    {
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportProgramExpendituresRecordsRepository)
            .Returns(_recordsRepositoryMock.Object);
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.HippotherapyProgramCategoriesRepository)
            .Returns(_hippotherapyProgramCategoriesRepositoryMock.Object);

        _hippotherapyProgramCategoriesRepositoryMock
            .Setup(repository =>
                repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgramCategory>>()))
            .ReturnsAsync(category);

        _recordsRepositoryMock
            .Setup(repository => repository.CreateAsync(It.IsAny<ReportProgramExpendituresRecord>()))
            .ReturnsAsync((ReportProgramExpendituresRecord record) => record);

        _recordsRepositoryMock
            .Setup(repository =>
                repository.RecordWithinSameCategoryWithSameYearExistsAsync(It.IsAny<ReportProgramExpendituresRecord>()))
            .ReturnsAsync(recordWithinSameCategoryWithSameYearExists);

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
