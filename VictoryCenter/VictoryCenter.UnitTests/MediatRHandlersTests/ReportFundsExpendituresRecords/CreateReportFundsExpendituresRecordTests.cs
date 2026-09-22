using AutoMapper;
using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;
using VictoryCenter.BLL.Interfaces.ReportFundsExpendituresRecordHelper;
using VictoryCenter.BLL.Notifications.ReportFunds;
using VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportFundsExpendituresRecords;

public class CreateReportFundsExpendituresRecordTests
{
    private const decimal ExchangeRate = 40.0m;

    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IReportFundsExpendituresRecordsRepository> _recordsRepositoryMock;
    private readonly Mock<IReportFundsExpendituresCategoriesRepository> _categoriesRepositoryMock;
    private readonly Mock<IReportFundsExpendituresRecordHelper> _helperMock;
    private readonly IValidator<CreateReportFundsExpendituresRecordCommand> _validator;

    private readonly CreateReportFundsExpendituresRecordDto _createDto = new()
    {
        CategoryId = 1,
        Type = ReportFundsExpendituresType.Income,
        ReportingYear = TimeProvider.System.GetUtcNow().Year,
        Amount = 100.50m,
        Currency = ReportFundsExpendituresCurrency.Uah
    };

    private readonly ReportFundsExpendituresCategory _category = new()
    {
        Id = 1,
        Name = "Income category",
        Type = ReportFundsExpendituresType.Income
    };

    private readonly ReportFundsExpendituresRecord _recordEntity = new()
    {
        Id = 1,
        CategoryId = 1,
        Type = ReportFundsExpendituresType.Income,
        ReportingYear = TimeProvider.System.GetUtcNow().Year,
    };

    private readonly ReportFundsExpendituresRecordDto _recordDto = new()
    {
        Id = 1,
        CategoryId = 1,
        Type = ReportFundsExpendituresType.Income,
        ReportingYear = TimeProvider.System.GetUtcNow().Year,
        AmountUah = 100.50m,
        AmountUsd = 2.5125m
    };

    public CreateReportFundsExpendituresRecordTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mediatorMock = new Mock<IMediator>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _recordsRepositoryMock = new Mock<IReportFundsExpendituresRecordsRepository>();
        _categoriesRepositoryMock = new Mock<IReportFundsExpendituresCategoriesRepository>();
        _helperMock = new Mock<IReportFundsExpendituresRecordHelper>();
        _validator = new CreateReportFundsExpendituresRecordValidator(
            new BaseReportFundsExpendituresRecordValidator(),
            TimeProvider.System);
    }

    [Fact]
    public async Task Handle_ShouldCreateRecord()
    {
        // Arrange
        SetupDependencies(category: _category, saveResult: 1);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new CreateReportFundsExpendituresRecordCommand(_createDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_recordDto.CategoryId, result.Value.CategoryId);
        Assert.Equal(_recordDto.AmountUah, result.Value.AmountUah);
        Assert.Equal(_createDto.Amount, _recordEntity.AmountUah);
        Assert.Equal(_createDto.Amount / ExchangeRate, _recordEntity.AmountUsd);
        _mediatorMock.Verify(
            mediator => mediator.Publish(
                It.IsAny<ReportFundsChangedNotification>(),
                It.Is<CancellationToken>(token => token == CancellationToken.None)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var invalidDto = _createDto with { CategoryId = 0 };
        SetupDependencies(category: _category, saveResult: 1);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new CreateReportFundsExpendituresRecordCommand(invalidDto),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSettingsAreInvalid()
    {
        // Arrange
        SetupDependencies(category: _category, saveResult: 1);
        _helperMock
            .Setup(h => h.GetAndValidateSettingsAsync())
            .ReturnsAsync(FluentResults.Result.Fail<ReportFundsExpendituresSettingsDto>("invalid settings"));
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new CreateReportFundsExpendituresRecordCommand(_createDto),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCategoryNotFound()
    {
        // Arrange
        SetupDependencies(category: null, saveResult: 1);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new CreateReportFundsExpendituresRecordCommand(_createDto),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(_createDto.CategoryId, typeof(ReportFundsExpendituresCategory)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCategoryTypeDoesNotMatchRecordType()
    {
        // Arrange
        var expenseCategory = new ReportFundsExpendituresCategory
        {
            Id = 1,
            Name = "Expense category",
            Type = ReportFundsExpendituresType.Expense
        };

        SetupDependencies(category: expenseCategory, saveResult: 1);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new CreateReportFundsExpendituresRecordCommand(_createDto),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ReportFundsExpendituresRecordConstants.CategoryTypeMustMatchRecordType, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCategoryAlreadyHasRecord()
    {
        // Arrange
        SetupDependencies(category: _category, saveResult: 1, existingRecordInCategory: _recordEntity);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new CreateReportFundsExpendituresRecordCommand(_createDto),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ReportFundsExpendituresRecordConstants.CategoryAlreadyHasRecord, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        // Arrange
        SetupDependencies(category: _category, saveResult: 0);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new CreateReportFundsExpendituresRecordCommand(_createDto),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(ReportFundsExpendituresRecord)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionOccurs()
    {
        // Arrange
        SetupDependencies(category: _category, saveResult: 1);
        _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new CreateReportFundsExpendituresRecordCommand(_createDto),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(ReportFundsExpendituresRecord)),
            result.Errors[0].Message);
    }

    private CreateReportFundsExpendituresRecordHandler CreateHandler() =>
        new(
            _mapperMock.Object,
            _mediatorMock.Object,
            _repositoryWrapperMock.Object,
            _validator,
            _helperMock.Object);

    private void SetupDependencies(
        ReportFundsExpendituresCategory? category,
        int saveResult,
        ReportFundsExpendituresRecord? existingRecordInCategory = null)
    {
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportFundsExpendituresRecordsRepository)
            .Returns(_recordsRepositoryMock.Object);
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportFundsExpendituresCategoriesRepository)
            .Returns(_categoriesRepositoryMock.Object);

        _helperMock
            .Setup(h => h.GetAndValidateSettingsAsync())
            .ReturnsAsync(FluentResults.Result.Ok(new ReportFundsExpendituresSettingsDto { ExchangeRate = ExchangeRate }));

        _helperMock
            .Setup(h => h.CalculateAmounts(It.IsAny<decimal>(), It.IsAny<ReportFundsExpendituresCurrency>(), It.IsAny<decimal>()))
            .Returns((decimal amount, ReportFundsExpendituresCurrency currency, decimal exchangeRate) =>
                currency == ReportFundsExpendituresCurrency.Usd
                    ? (AmountUah: amount * exchangeRate, AmountUsd: amount)
                    : (AmountUah: amount, AmountUsd: amount / exchangeRate));

        _categoriesRepositoryMock
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ReportFundsExpendituresCategory>>()))
            .ReturnsAsync(category);

        _recordsRepositoryMock
            .Setup(repository => repository.CreateAsync(It.IsAny<ReportFundsExpendituresRecord>()))
            .ReturnsAsync((ReportFundsExpendituresRecord record) => record);

        _recordsRepositoryMock
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ReportFundsExpendituresRecord>>()))
            .ReturnsAsync(existingRecordInCategory);

        _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(saveResult);
        _mediatorMock
            .Setup(mediator => mediator.Publish(
                It.IsAny<ReportFundsChangedNotification>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(mapper => mapper.Map<ReportFundsExpendituresRecord>(It.IsAny<CreateReportFundsExpendituresRecordDto>()))
            .Returns(_recordEntity);
        _mapperMock
            .Setup(mapper => mapper.Map<ReportFundsExpendituresRecordDto>(It.IsAny<ReportFundsExpendituresRecord>()))
            .Returns(_recordDto);
    }
}
