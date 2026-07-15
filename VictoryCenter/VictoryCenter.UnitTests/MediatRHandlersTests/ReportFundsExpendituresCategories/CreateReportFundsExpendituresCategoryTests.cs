using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresCategories.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresCategories;
using VictoryCenter.BLL.Validators.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportFundsExpendituresCategories;

public class CreateReportFundsExpendituresCategoryTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IReportFundsExpendituresCategoriesRepository> _categoriesRepositoryMock;
    private readonly IValidator<CreateReportFundsExpendituresCategoryCommand> _validator;

    private readonly CreateReportFundsExpendituresCategoryDto _createDto = new()
    {
        Name = "Income category",
        Type = ReportFundsExpendituresType.Income
    };

    private readonly ReportFundsExpendituresCategory _categoryEntity = new()
    {
        Id = 1,
        Name = "Income category",
        Type = ReportFundsExpendituresType.Income
    };

    private readonly ReportFundsExpendituresCategoryDto _categoryDto = new()
    {
        Id = 1,
        Name = "Income category",
        Type = ReportFundsExpendituresType.Income
    };

    public CreateReportFundsExpendituresCategoryTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _categoriesRepositoryMock = new Mock<IReportFundsExpendituresCategoriesRepository>();
        _validator = new CreateReportFundsExpendituresCategoryValidator(
            new BaseReportFundsExpendituresCategoryValidator());
    }

    [Fact]
    public async Task Handle_ShouldCreateCategory()
    {
        // Arrange
        SetupDependencies(duplicateCategoriesCount: 0, saveResult: 1);
        var handler = new CreateReportFundsExpendituresCategoryHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new CreateReportFundsExpendituresCategoryCommand(_createDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_categoryDto.Name, result.Value.Name);
        Assert.Equal(_categoryDto.Type, result.Value.Type);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_ShouldFail_WhenNameIsInvalid(string? name)
    {
        // Arrange
        var invalidDto = _createDto with { Name = name! };
        var handler = new CreateReportFundsExpendituresCategoryHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new CreateReportFundsExpendituresCategoryCommand(invalidDto),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDuplicateCategoryExists()
    {
        // Arrange
        SetupDependencies(duplicateCategoriesCount: 1, saveResult: 1);

        var handler = new CreateReportFundsExpendituresCategoryHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new CreateReportFundsExpendituresCategoryCommand(_createDto),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ReportFundsExpendituresCategoryConstants.DuplicateCategoryName, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenNameIsReservedAndTypeIsExpense()
    {
        // Arrange
        var reservedDto = _createDto with { Name = "Програмні тест 2", Type = ReportFundsExpendituresType.Expense };
        SetupDependencies(duplicateCategoriesCount: 0, saveResult: 1);

        var handler = new CreateReportFundsExpendituresCategoryHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new CreateReportFundsExpendituresCategoryCommand(reservedDto),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ReportFundsExpendituresCategoryConstants.ReservedCategoryName, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldCreateCategory_WhenNameIsReservedButTypeIsIncome()
    {
        // Arrange
        var reservedNameIncomeDto = _createDto with { Name = "Програмні тест 2", Type = ReportFundsExpendituresType.Income };
        SetupDependencies(duplicateCategoriesCount: 0, saveResult: 1);

        var handler = new CreateReportFundsExpendituresCategoryHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new CreateReportFundsExpendituresCategoryCommand(reservedNameIncomeDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        // Arrange
        SetupDependencies(duplicateCategoriesCount: 0, saveResult: 0);

        var handler = new CreateReportFundsExpendituresCategoryHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new CreateReportFundsExpendituresCategoryCommand(_createDto),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(ReportFundsExpendituresCategory)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionOccurs()
    {
        // Arrange
        SetupDependencies(duplicateCategoriesCount: 0, saveResult: 1);
        _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException());

        var handler = new CreateReportFundsExpendituresCategoryHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _validator);

        // Act
        var result = await handler.Handle(
            new CreateReportFundsExpendituresCategoryCommand(_createDto),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(ReportFundsExpendituresCategory)),
            result.Errors[0].Message);
    }

    private void SetupDependencies(int duplicateCategoriesCount, int saveResult)
    {
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportFundsExpendituresCategoriesRepository)
            .Returns(_categoriesRepositoryMock.Object);

        _categoriesRepositoryMock
            .Setup(repository => repository.CountAsync(It.IsAny<QueryOptions<ReportFundsExpendituresCategory>>()))
            .ReturnsAsync(duplicateCategoriesCount);

        _categoriesRepositoryMock
            .Setup(repository => repository.CreateAsync(It.IsAny<ReportFundsExpendituresCategory>()))
            .ReturnsAsync((ReportFundsExpendituresCategory category) => category);

        _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(saveResult);

        _mapperMock.Setup(mapper => mapper.Map<ReportFundsExpendituresCategory>(It.IsAny<CreateReportFundsExpendituresCategoryDto>()))
            .Returns(_categoryEntity);
        _mapperMock.Setup(mapper => mapper.Map<ReportFundsExpendituresCategoryDto>(It.IsAny<ReportFundsExpendituresCategory>()))
            .Returns(_categoryDto);
    }
}
