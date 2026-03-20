using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresCategories;
using VictoryCenter.BLL.Queries.Admin.ReportFundsExpendituresCategories.GetAll;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportFundsExpendituresCategories;

public class GetAllReportFundsExpendituresCategoriesTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IReportFundsExpendituresCategoriesRepository> _categoriesRepositoryMock;

    private readonly List<ReportFundsExpendituresCategory> _categories =
    [
        new()
        {
            Id = 1,
            Name = "Income category",
            Type = ReportFundsExpendituresType.Income
        },
        new()
        {
            Id = 2,
            Name = "Expense category",
            Type = ReportFundsExpendituresType.Expense
        }

    ];

    private readonly List<ReportFundsExpendituresCategoryDto> _categoryDtos =
    [
        new()
        {
            Id = 1,
            Name = "Income category",
            Type = ReportFundsExpendituresType.Income
        },
        new()
        {
            Id = 2,
            Name = "Expense category",
            Type = ReportFundsExpendituresType.Expense
        }

    ];

    public GetAllReportFundsExpendituresCategoriesTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _categoriesRepositoryMock = new Mock<IReportFundsExpendituresCategoriesRepository>();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllCategories()
    {
        // Arrange
        SetupDependencies(_categories, _categoryDtos);
        var handler = new GetAllReportFundsExpendituresCategoriesHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(
            new GetAllReportFundsExpendituresCategoriesQuery(),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_categoryDtos.Count, result.Value.Count());
        Assert.Equal(_categoryDtos[0].Name, result.Value.ElementAt(0).Name);
        Assert.Equal(_categoryDtos[1].Name, result.Value.ElementAt(1).Name);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyCollection_WhenNoCategoriesExist()
    {
        // Arrange
        SetupDependencies([], []);
        var handler = new GetAllReportFundsExpendituresCategoriesHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object);

        // Act
        var result = await handler.Handle(
            new GetAllReportFundsExpendituresCategoriesQuery(),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    private void SetupDependencies(
        IEnumerable<ReportFundsExpendituresCategory> categories,
        IEnumerable<ReportFundsExpendituresCategoryDto> categoryDtos)
    {
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportFundsExpendituresCategoriesRepository)
            .Returns(_categoriesRepositoryMock.Object);

        _categoriesRepositoryMock
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<ReportFundsExpendituresCategory>>()))
            .ReturnsAsync(categories);

        _mapperMock
            .Setup(mapper => mapper.Map<IEnumerable<ReportFundsExpendituresCategoryDto>>(
                It.IsAny<IEnumerable<ReportFundsExpendituresCategory>>()))
            .Returns(categoryDtos);
    }
}
