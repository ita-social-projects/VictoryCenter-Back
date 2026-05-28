using AutoMapper;
using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
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
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IReportFundsExpendituresRecordsRepository> _recordsRepositoryMock;
    private readonly Mock<IReportFundsExpendituresCategoriesRepository> _categoriesRepositoryMock;
    private readonly IValidator<CreateReportFundsExpendituresRecordCommand> _validator;

    private readonly CreateReportFundsExpendituresRecordDto _createDto = new()
    {
        CategoryId = 1,
        Type = ReportFundsExpendituresType.Income,
        ReportingYear = TimeProvider.System.GetUtcNow().Year,
        AmountUah = 100.50m,
        AmountUsd = 25.25m
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
        AmountUah = 100.50m,
        AmountUsd = 25.25m
    };

    private readonly ReportFundsExpendituresRecordDto _recordDto = new()
    {
        Id = 1,
        CategoryId = 1,
        Type = ReportFundsExpendituresType.Income,
        ReportingYear = TimeProvider.System.GetUtcNow().Year,
        AmountUah = 100.50m,
        AmountUsd = 25.25m
    };

    public CreateReportFundsExpendituresRecordTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mediatorMock = new Mock<IMediator>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _recordsRepositoryMock = new Mock<IReportFundsExpendituresRecordsRepository>();
        _categoriesRepositoryMock = new Mock<IReportFundsExpendituresCategoriesRepository>();
        _validator = new CreateReportFundsExpendituresRecordValidator(
            new BaseReportFundsExpendituresRecordValidator(),
            TimeProvider.System);
    }

    [Fact]
    public async Task Handle_ShouldCreateRecord()
    {
        // Arrange
        SetupDependencies(category: _category, saveResult: 1);
        var handler = new CreateReportFundsExpendituresRecordHandler(
            _mapperMock.Object,
            _mediatorMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new CreateReportFundsExpendituresRecordCommand(_createDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_recordDto.CategoryId, result.Value.CategoryId);
        Assert.Equal(_recordDto.AmountUah, result.Value.AmountUah);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var invalidDto = _createDto with { CategoryId = 0 };
        var handler = new CreateReportFundsExpendituresRecordHandler(
            _mapperMock.Object,
            _mediatorMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new CreateReportFundsExpendituresRecordCommand(invalidDto),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCategoryNotFound()
    {
        // Arrange
        SetupDependencies(category: null, saveResult: 1);
        var handler = new CreateReportFundsExpendituresRecordHandler(
            _mapperMock.Object,
            _mediatorMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

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
        var handler = new CreateReportFundsExpendituresRecordHandler(
            _mapperMock.Object,
            _mediatorMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

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
        var handler = new CreateReportFundsExpendituresRecordHandler(
            _mapperMock.Object,
            _mediatorMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

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
        var handler = new CreateReportFundsExpendituresRecordHandler(
            _mapperMock.Object,
            _mediatorMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

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

        var handler = new CreateReportFundsExpendituresRecordHandler(
            _mapperMock.Object,
            _mediatorMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

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

    private void SetupDependencies(
        ReportFundsExpendituresCategory? category,
        int saveResult,
        ReportFundsExpendituresRecord? existingRecordInCategory = null)
    {
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportFundsExpendituresRecordsRepository)
            .Returns(_recordsRepositoryMock.Object);
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportFundsExpendituresCategoriesRepository)
            .Returns(_categoriesRepositoryMock.Object);

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

        _mapperMock
            .Setup(mapper => mapper.Map<ReportFundsExpendituresRecord>(It.IsAny<CreateReportFundsExpendituresRecordDto>()))
            .Returns(_recordEntity);
        _mapperMock
            .Setup(mapper => mapper.Map<ReportFundsExpendituresRecordDto>(It.IsAny<ReportFundsExpendituresRecord>()))
            .Returns(_recordDto);
    }
}
