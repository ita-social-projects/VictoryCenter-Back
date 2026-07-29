using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.HippotherapyProgramCategories;
using VictoryCenter.DAL.Repositories.Interfaces.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Repositories.Options;
using MediatR;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportProgramExpendituresRecords;

public class UpdateReportProgramExpendituresRecordTests
{
    private readonly UpdateReportProgramExpendituresRecordDto _updateDto = new()
    {
        HippotherapyProgramCategoryId = 2,
        AmountUah = 150.75m,
        AmountUsd = 40.50m
    };

    private readonly Mock<IMapper> _mapperMock;

    private readonly ReportProgramExpendituresRecordDto _recordDto = new()
    {
        Id = 1,
        HippotherapyProgramCategoryId = 2,
        ReportingYear = ReportProgramExpendituresRecordConstants.ReportingYearMinValue,
        AmountUah = 150.75m,
        AmountUsd = 40.50m
    };

    private readonly ReportProgramExpendituresRecord _recordEntity;

    private readonly Mock<IHippotherapyProgramCategoriesRepository> _hippotherapyProgramCategoriesRepositoryMock;
    private readonly Mock<IReportProgramExpendituresRecordsRepository> _recordsRepositoryMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly IValidator<UpdateReportProgramExpendituresRecordCommand> _validator;

    public UpdateReportProgramExpendituresRecordTests()
    {
        _recordEntity = new ReportProgramExpendituresRecord
        {
            Id = 1,
            HippotherapyProgramCategoryId = 1,
            ReportingYear = ReportProgramExpendituresRecordConstants.ReportingYearMinValue,
            AmountUah = 100.50m,
            AmountUsd = 25.25m
        };

        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _hippotherapyProgramCategoriesRepositoryMock = new Mock<IHippotherapyProgramCategoriesRepository>();
        _recordsRepositoryMock = new Mock<IReportProgramExpendituresRecordsRepository>();
        _mediatorMock = new Mock<IMediator>();
        _validator = new UpdateReportProgramExpendituresRecordCommandValidator(_repositoryWrapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateRecord_WhenCategoryNotChanged()
    {
        // Arrange
        var updateDto = _updateDto with { HippotherapyProgramCategoryId = 1 };
        SetupDependencies(_recordEntity, 1);
        var handler = new UpdateReportProgramExpendituresRecordHandler(
            _validator,
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _mediatorMock.Object);

        // Act
        var result = await handler.Handle(
            new UpdateReportProgramExpendituresRecordCommand(1, updateDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _recordsRepositoryMock.Verify(x => x.Update(It.IsAny<ReportProgramExpendituresRecord>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUpdateRecord_WhenCategoryChangedAndNoConflict()
    {
        // Arrange
        SetupDependencies(_recordEntity, 1);
        var handler = new UpdateReportProgramExpendituresRecordHandler(
            _validator,
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _mediatorMock.Object);

        // Act
        var result = await handler.Handle(
            new UpdateReportProgramExpendituresRecordCommand(1, _updateDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _recordsRepositoryMock.Verify(x => x.RecordWithinSameCategoryWithSameYearExistsAsync(It.IsAny<ReportProgramExpendituresRecord>()), Times.Once);
        _recordsRepositoryMock.Verify(x => x.Update(It.IsAny<ReportProgramExpendituresRecord>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var invalidDto = _updateDto with { HippotherapyProgramCategoryId = 0 };
        var handler = new UpdateReportProgramExpendituresRecordHandler(
            _validator,
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _mediatorMock.Object);

        // Act
        var result = await handler.Handle(
            new UpdateReportProgramExpendituresRecordCommand(1, invalidDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.PropertyMustBePositive(
                nameof(ReportProgramExpendituresRecord.HippotherapyProgramCategoryId)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenRecordNotFound()
    {
        // Arrange
        SetupDependencies(null, 1);
        var handler = new UpdateReportProgramExpendituresRecordHandler(
            _validator,
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _mediatorMock.Object);

        // Act
        var result = await handler.Handle(
            new UpdateReportProgramExpendituresRecordCommand(1, _updateDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(
                1,
                typeof(ReportProgramExpendituresRecord)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenMovedToDifferentCategoryAndRecordAlreadyExists()
    {
        // Arrange
        SetupDependencies(_recordEntity, 1, conflictExists: true);
        var handler = new UpdateReportProgramExpendituresRecordHandler(
            _validator,
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _mediatorMock.Object);

        // Act
        var result = await handler.Handle(
            new UpdateReportProgramExpendituresRecordCommand(1, _updateDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ReportProgramExpendituresRecordConstants.ProgramCategoryAlreadyHasRecordForSpecifiedYear(
                _updateDto.HippotherapyProgramCategoryId, _recordEntity.ReportingYear),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        // Arrange
        SetupDependencies(_recordEntity, 0);
        var handler = new UpdateReportProgramExpendituresRecordHandler(
            _validator,
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _mediatorMock.Object);

        // Act
        var result = await handler.Handle(
            new UpdateReportProgramExpendituresRecordCommand(1, _updateDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(ReportProgramExpendituresRecord)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionOccurs()
    {
        // Arrange
        SetupDependencies(_recordEntity, 1);
        _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());

        var handler = new UpdateReportProgramExpendituresRecordHandler(
            _validator,
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _mediatorMock.Object);

        // Act
        var result = await handler.Handle(
            new UpdateReportProgramExpendituresRecordCommand(1, _updateDto),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(ReportProgramExpendituresRecord)),
            result.Errors[0].Message);
    }

    private void SetupDependencies(
        ReportProgramExpendituresRecord? existingRecord,
        int saveResult,
        bool conflictExists = false)
    {
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportProgramExpendituresRecordsRepository)
            .Returns(_recordsRepositoryMock.Object);
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.HippotherapyProgramCategoriesRepository)
            .Returns(_hippotherapyProgramCategoriesRepositoryMock.Object);

        _hippotherapyProgramCategoriesRepositoryMock
            .Setup(repository =>
                repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgramCategory>>()))
            .ReturnsAsync(new HippotherapyProgramCategory { Id = 1 });

        _recordsRepositoryMock
            .Setup(repository =>
                repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ReportProgramExpendituresRecord>>()))
            .ReturnsAsync(existingRecord);

        _recordsRepositoryMock
            .Setup(repository =>
                repository.RecordWithinSameCategoryWithSameYearExistsAsync(It.IsAny<ReportProgramExpendituresRecord>()))
            .ReturnsAsync(conflictExists);

        _recordsRepositoryMock
            .Setup(repository => repository.Update(It.IsAny<ReportProgramExpendituresRecord>()));

        _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(saveResult);

        _mapperMock
            .Setup(mapper =>
                mapper.Map(It.IsAny<UpdateReportProgramExpendituresRecordDto>(), It.IsAny<ReportProgramExpendituresRecord>()))
            .Callback<UpdateReportProgramExpendituresRecordDto, ReportProgramExpendituresRecord>((src, dest) =>
            {
                if (dest != null)
                {
                    dest.HippotherapyProgramCategoryId = src.HippotherapyProgramCategoryId;
                }
            })
            .Returns((UpdateReportProgramExpendituresRecordDto src, ReportProgramExpendituresRecord dest) => dest);

        _mapperMock
            .Setup(mapper =>
                mapper.Map<ReportProgramExpendituresRecordDto>(It.IsAny<ReportProgramExpendituresRecord>()))
            .Returns(_recordDto);
    }
}
