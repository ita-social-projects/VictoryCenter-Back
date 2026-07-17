using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresCategories.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportFundsExpendituresCategories;

public class DeleteReportFundsExpendituresCategoryTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IReportFundsExpendituresCategoriesRepository> _categoriesRepositoryMock;

    private readonly ReportFundsExpendituresCategory _category = new()
    {
        Id = 1,
        Name = "Income category",
        Type = ReportFundsExpendituresType.Income
    };

    public DeleteReportFundsExpendituresCategoryTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _categoriesRepositoryMock = new Mock<IReportFundsExpendituresCategoriesRepository>();
    }

    [Fact]
    public async Task Handle_ShouldDeleteCategory()
    {
        // Arrange
        SetupDependencies(_category, saveResult: 1);
        var handler = new DeleteReportFundsExpendituresCategoryHandler(_repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(
            new DeleteReportFundsExpendituresCategoryCommand(_category.Id),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_category.Id, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCategoryNotFound()
    {
        // Arrange
        SetupDependencies(null, saveResult: 1);
        var handler = new DeleteReportFundsExpendituresCategoryHandler(_repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(
            new DeleteReportFundsExpendituresCategoryCommand(999),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.NotFound(999, typeof(ReportFundsExpendituresCategory)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCategoryHasRecords()
    {
        // Arrange
        var categoryWithRecords = new ReportFundsExpendituresCategory
        {
            Id = 1,
            Name = "Income category",
            Type = ReportFundsExpendituresType.Income,
            Records = [new ReportFundsExpendituresRecord()]
        };

        SetupDependencies(categoryWithRecords, saveResult: 1);
        var handler = new DeleteReportFundsExpendituresCategoryHandler(_repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(
            new DeleteReportFundsExpendituresCategoryCommand(categoryWithRecords.Id),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ReportFundsExpendituresCategoryConstants.CantDeleteCategoryWhileAssociatedWithAnyRecord,
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCategoryIsReserved()
    {
        // Arrange
        var reservedCategory = new ReportFundsExpendituresCategory
        {
            Id = 1,
            Name = "Програмні тест 2",
            Type = ReportFundsExpendituresType.Expense
        };

        SetupDependencies(reservedCategory, saveResult: 1);
        var handler = new DeleteReportFundsExpendituresCategoryHandler(_repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(
            new DeleteReportFundsExpendituresCategoryCommand(reservedCategory.Id),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ReportFundsExpendituresCategoryConstants.CantDeleteReservedCategory,
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldDeleteCategory_WhenReservedNameButIncomeType()
    {
        // Arrange
        var incomeCategory = new ReportFundsExpendituresCategory
        {
            Id = 1,
            Name = "Програмні тест 2",
            Type = ReportFundsExpendituresType.Income
        };

        SetupDependencies(incomeCategory, saveResult: 1);
        var handler = new DeleteReportFundsExpendituresCategoryHandler(_repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(
            new DeleteReportFundsExpendituresCategoryCommand(incomeCategory.Id),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        // Arrange
        SetupDependencies(_category, saveResult: 0);
        var handler = new DeleteReportFundsExpendituresCategoryHandler(_repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(
            new DeleteReportFundsExpendituresCategoryCommand(_category.Id),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntity(typeof(ReportFundsExpendituresCategory)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionOccurs()
    {
        // Arrange
        SetupDependencies(
            _category,
            saveResult: 1,
            saveException: new DbUpdateException("Database error"));
        var handler = new DeleteReportFundsExpendituresCategoryHandler(_repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(
            new DeleteReportFundsExpendituresCategoryCommand(_category.Id),
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToDeleteEntity(typeof(ReportFundsExpendituresCategory)),
            result.Errors[0].Message);
    }

    private void SetupDependencies(ReportFundsExpendituresCategory? category, int saveResult, Exception? saveException = null)
    {
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportFundsExpendituresCategoriesRepository)
            .Returns(_categoriesRepositoryMock.Object);

        _categoriesRepositoryMock
            .Setup(repository => repository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ReportFundsExpendituresCategory>>()))
            .ReturnsAsync(category);

        _categoriesRepositoryMock
            .Setup(repository => repository.Delete(It.IsAny<ReportFundsExpendituresCategory>()));

        if (saveException is null)
        {
            _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync()).ReturnsAsync(saveResult);
            return;
        }

        _repositoryWrapperMock.Setup(wrapper => wrapper.SaveChangesAsync()).ThrowsAsync(saveException);
    }
}
