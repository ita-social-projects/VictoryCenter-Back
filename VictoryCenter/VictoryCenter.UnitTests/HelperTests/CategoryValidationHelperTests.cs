using FluentResults;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.HippotherapyProgramCategories;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.HelperTests;

public class CategoryValidationHelperTests
{
    private readonly Mock<IHippotherapyProgramCategoriesRepository> _categoryRepoMock;

    public CategoryValidationHelperTests()
    {
        _categoryRepoMock = new Mock<IHippotherapyProgramCategoriesRepository>();
    }

    [Fact]
    public async Task ValidateAndGetCategoriesAsync_AllCategoriesFound_ReturnsOkWithCategories()
    {
        // Arrange
        var requestedIds = new List<long> { 1, 2, 3 };
        var categories = new List<HippotherapyProgramCategory>
        {
            MakeCategory(1, "Category 1"),
            MakeCategory(2, "Category 2"),
            MakeCategory(3, "Category 3")
        };

        _categoryRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<HippotherapyProgramCategory>>()))
            .ReturnsAsync(categories);

        // Act
        Result<ICollection<HippotherapyProgramCategory>> result =
            await CategoryValidationHelper.ValidateAndGetCategoriesAsync(_categoryRepoMock.Object, requestedIds);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(3, result.Value.Count);
        Assert.Equal(categories, result.Value);

        _categoryRepoMock.Verify(
            r => r.GetAllAsync(
                It.Is<QueryOptions<HippotherapyProgramCategory>>(opts => VerifyQueryOptions(opts, requestedIds))),
            Times.Once);
    }

    [Fact]
    public async Task ValidateAndGetCategoriesAsync_SomeCategoriesMissing_ReturnsFailedWithMissingIds()
    {
        // Arrange
        var requestedIds = new List<long> { 1, 2, 3, 4 };
        var foundCategories = new List<HippotherapyProgramCategory>
        {
            MakeCategory(1, "Category 1"),
            MakeCategory(3, "Category 3")
        };

        _categoryRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<HippotherapyProgramCategory>>()))
            .ReturnsAsync(foundCategories);

        var expectedMessage = ErrorMessagesConstants.NotFound([2, 4], typeof(HippotherapyProgramCategory));

        // Act
        Result<ICollection<HippotherapyProgramCategory>> result =
            await CategoryValidationHelper.ValidateAndGetCategoriesAsync(_categoryRepoMock.Object, requestedIds);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Null(result.ValueOrDefault);
        Assert.Single(result.Errors);
        Assert.Equal(expectedMessage, result.Errors[0].Message);

        _categoryRepoMock.Verify(
            r => r.GetAllAsync(
                It.Is<QueryOptions<HippotherapyProgramCategory>>(opts => VerifyQueryOptions(opts, requestedIds))),
            Times.Once);
    }

    [Fact]
    public async Task ValidateAndGetCategoriesAsync_NoCategoriesFound_ReturnsFailedWithAllIds()
    {
        // Arrange
        var requestedIds = new List<long> { 10, 20 };
        var emptyList = new List<HippotherapyProgramCategory>();

        _categoryRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<HippotherapyProgramCategory>>()))
            .ReturnsAsync(emptyList);

        var expectedMessage = ErrorMessagesConstants.NotFound([10, 20], typeof(HippotherapyProgramCategory));

        // Act
        Result<ICollection<HippotherapyProgramCategory>> result =
            await CategoryValidationHelper.ValidateAndGetCategoriesAsync(_categoryRepoMock.Object, requestedIds);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Null(result.ValueOrDefault);
        Assert.Single(result.Errors);
        Assert.Equal(expectedMessage, result.Errors[0].Message);
    }

    [Fact]
    public async Task ValidateAndGetCategoriesAsync_EmptyRequestedIds_ReturnsOkWithEmptyCollection()
    {
        // Arrange
        var requestedIds = new List<long>();

        // Act
        Result<ICollection<HippotherapyProgramCategory>> result =
            await CategoryValidationHelper.ValidateAndGetCategoriesAsync(_categoryRepoMock.Object, requestedIds);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
        _categoryRepoMock.Verify(
            repository => repository.GetAllAsync(It.IsAny<QueryOptions<HippotherapyProgramCategory>>()),
            Times.Never);
    }

    private static HippotherapyProgramCategory MakeCategory(long id, string name)
    {
        return new HippotherapyProgramCategory
        {
            Id = id,
            CreatedAt = DateTimeOffset.UtcNow,
            Name = name,
            Programs = []
        };
    }

    private static bool VerifyQueryOptions(
        QueryOptions<HippotherapyProgramCategory>? opts,
        IEnumerable<long> expectedIds)
    {
        if (opts is null)
        {
            return false;
        }

        if (opts.AsNoTracking)
        {
            return false;
        }

        if (opts.Filter is null)
        {
            return false;
        }

        var filter = opts.Filter.Compile();
        var expectedIdsList = expectedIds.ToList();

        foreach (var id in expectedIdsList)
        {
            if (!filter(new HippotherapyProgramCategory { Id = id }))
            {
                return false;
            }
        }

        if (filter(new HippotherapyProgramCategory { Id = 99999 }))
        {
            return false;
        }

        return true;
    }
}
