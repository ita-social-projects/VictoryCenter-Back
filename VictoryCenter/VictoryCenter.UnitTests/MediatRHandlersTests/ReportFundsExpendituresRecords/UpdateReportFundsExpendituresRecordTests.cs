using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;
using VictoryCenter.BLL.Interfaces.UpdateReportFundsExpendituresRecordHelper;
using VictoryCenter.BLL.Notifications.ReportFunds;
using VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportFundsExpendituresRecords;

public class UpdateReportFundsExpendituresRecordTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IReportFundsExpendituresRecordsRepository> _recordsRepositoryMock;
    private readonly Mock<IUpdateReportFundsExpendituresRecordHelper> _helperMock;
    private readonly IValidator<UpdateReportFundsExpendituresRecordCommand> _validator;

    private readonly ReportFundsExpendituresRecord _existingRecord = new()
    {
        Id = 1,
        CategoryId = 1,
        Type = ReportFundsExpendituresType.Income,
        ReportingYear = 2025,
        AmountUah = 100.50m,
        AmountUsd = 25.25m
    };

    private readonly UpdateReportFundsExpendituresRecordDto _updateDto = new()
    {
        CategoryId = 1,
        AmountUah = 200.10m,
        AmountUsd = 50.10m
    };

    private readonly ReportFundsExpendituresRecordDto _recordDto = new()
    {
        Id = 1,
        CategoryId = 1,
        Type = ReportFundsExpendituresType.Income,
        ReportingYear = 2025,
        AmountUah = 200.10m,
        AmountUsd = 50.10m
    };

    public UpdateReportFundsExpendituresRecordTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mediatorMock = new Mock<IMediator>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _recordsRepositoryMock = new Mock<IReportFundsExpendituresRecordsRepository>();
        _helperMock = new Mock<IUpdateReportFundsExpendituresRecordHelper>();
        _validator = new UpdateReportFundsExpendituresRecordValidator(new BaseReportFundsExpendituresRecordValidator());
    }

    [Fact]
    public async Task Handle_ShouldUpdateRecord_WhenCategoryIsNotChanged()
    {
        // Arrange
        SetupDependencies(recordToUpdate: _existingRecord, saveResult: 1);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new UpdateReportFundsExpendituresRecordCommand(_updateDto, _existingRecord.Id),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_recordDto.AmountUah, result.Value.AmountUah);
        Assert.Equal(_recordDto.AmountUsd, result.Value.AmountUsd);
        _mediatorMock.Verify(
            mediator => mediator.Publish(
                It.IsAny<ReportFundsChangedNotification>(),
                It.Is<CancellationToken>(token => token == CancellationToken.None)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUpdateRecord_WhenCategoryIsChangedAndTypeMatches()
    {
        // Arrange
        var changedCategoryDto = _updateDto with { CategoryId = 2 };
        SetupDependencies(recordToUpdate: _existingRecord, saveResult: 1);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new UpdateReportFundsExpendituresRecordCommand(changedCategoryDto, _existingRecord.Id),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var invalidDto = _updateDto with { CategoryId = 0 };
        SetupDependencies(recordToUpdate: _existingRecord, saveResult: 1);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new UpdateReportFundsExpendituresRecordCommand(invalidDto, _existingRecord.Id),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenRecordNotFound()
    {
        // Arrange
        SetupDependencies(recordToUpdate: null, saveResult: 1);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new UpdateReportFundsExpendituresRecordCommand(_updateDto, 999),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(999, typeof(ReportFundsExpendituresRecord)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenNewCategoryNotFound()
    {
        // Arrange
        var changedCategoryDto = _updateDto with { CategoryId = 2 };
        SetupDependencies(recordToUpdate: _existingRecord, saveResult: 1);

        _helperMock
            .Setup(h => h.ValidateCategoryChangeAsync(It.IsAny<ReportFundsExpendituresRecord>(), It.IsAny<long>(), It.IsAny<long>()))
            .ReturnsAsync(Result.Fail(ErrorMessagesConstants.NotFound(2, typeof(ReportFundsExpendituresCategory))));

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new UpdateReportFundsExpendituresRecordCommand(changedCategoryDto, _existingRecord.Id),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(2, typeof(ReportFundsExpendituresCategory)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCategoryTypeDoesNotMatchRecordType()
    {
        // Arrange
        var changedCategoryDto = _updateDto with { CategoryId = 2 };
        SetupDependencies(recordToUpdate: _existingRecord, saveResult: 1);

        _helperMock
            .Setup(h => h.ValidateCategoryChangeAsync(It.IsAny<ReportFundsExpendituresRecord>(), It.IsAny<long>(), It.IsAny<long>()))
            .ReturnsAsync(Result.Fail(ReportFundsExpendituresRecordConstants.CategoryTypeMustMatchRecordType));

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new UpdateReportFundsExpendituresRecordCommand(changedCategoryDto, _existingRecord.Id),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ReportFundsExpendituresRecordConstants.CategoryTypeMustMatchRecordType, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenNewCategoryAlreadyHasRecord()
    {
        // Arrange
        var changedCategoryDto = _updateDto with { CategoryId = 2 };
        SetupDependencies(recordToUpdate: _existingRecord, saveResult: 1);

        _helperMock
            .Setup(h => h.ValidateCategoryChangeAsync(It.IsAny<ReportFundsExpendituresRecord>(), It.IsAny<long>(), It.IsAny<long>()))
            .ReturnsAsync(Result.Fail(ReportFundsExpendituresRecordConstants.CategoryAlreadyHasRecord));

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new UpdateReportFundsExpendituresRecordCommand(changedCategoryDto, _existingRecord.Id),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ReportFundsExpendituresRecordConstants.CategoryAlreadyHasRecord, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        // Arrange
        SetupDependencies(recordToUpdate: _existingRecord, saveResult: 0);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new UpdateReportFundsExpendituresRecordCommand(_updateDto, _existingRecord.Id),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(ReportFundsExpendituresRecord)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionOccurs()
    {
        // Arrange
        SetupDependencies(recordToUpdate: _existingRecord, saveResult: 1);
        _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new UpdateReportFundsExpendituresRecordCommand(_updateDto, _existingRecord.Id),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(ReportFundsExpendituresRecord)),
            result.Errors[0].Message);
    }

    private void SetupDependencies(ReportFundsExpendituresRecord? recordToUpdate, int saveResult)
    {
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportFundsExpendituresRecordsRepository)
            .Returns(_recordsRepositoryMock.Object);

        _helperMock
            .Setup(h => h.GetAndValidateSettingsAsync())
            .ReturnsAsync(Result.Ok(new ReportFundsExpendituresSettingsDto { ExchangeRate = 40.0m }));

        _helperMock
            .Setup(h => h.ValidateCategoryChangeAsync(It.IsAny<ReportFundsExpendituresRecord>(), It.IsAny<long>(), It.IsAny<long>()))
            .ReturnsAsync(Result.Ok());

        _recordsRepositoryMock
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ReportFundsExpendituresRecord>>()))
            .ReturnsAsync((QueryOptions<ReportFundsExpendituresRecord>? options) =>
            {
                var records = new List<ReportFundsExpendituresRecord>();

                if (recordToUpdate is not null)
                {
                    records.Add(recordToUpdate);
                }

                var filter = options?.Filter;
                return filter is null
                    ? records.FirstOrDefault()
                    : records.FirstOrDefault(filter.Compile());
            });

        _recordsRepositoryMock.Setup(repository => repository.Update(It.IsAny<ReportFundsExpendituresRecord>()));
        _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(saveResult);

        _mediatorMock
            .Setup(mediator => mediator.Publish(
                It.IsAny<ReportFundsChangedNotification>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(mapper => mapper.Map(
                It.IsAny<UpdateReportFundsExpendituresRecordDto>(),
                It.IsAny<ReportFundsExpendituresRecord>()))
            .Callback<UpdateReportFundsExpendituresRecordDto, ReportFundsExpendituresRecord>(
                (dto, record) =>
                {
                    record.CategoryId = dto.CategoryId;
                    record.AmountUah = dto.AmountUah;
                    record.AmountUsd = dto.AmountUsd;
                })
            .Returns((UpdateReportFundsExpendituresRecordDto _, ReportFundsExpendituresRecord record) => record);

        _mapperMock
            .Setup(mapper => mapper.Map<ReportFundsExpendituresRecordDto>(It.IsAny<ReportFundsExpendituresRecord>()))
            .Returns(_recordDto);
    }

    private UpdateReportFundsExpendituresRecordHandler CreateHandler() =>
        new(
            _mapperMock.Object,
            _mediatorMock.Object,
            _repositoryWrapperMock.Object,
            _validator,
            _helperMock.Object
        );
}
