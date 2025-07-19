using Moq;
using VictoryCenter.BLL.Commands.Admin.Categories.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Categories;

public class DeleteCategoryTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly Category _testExistingCategory = new ()
    {
        Id = 1,
        Name = "Test name",
        Description = "Test description",
        CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Local),
    };

    public DeleteCategoryTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldDeleteCategory()
    {
        SetupRepositoryWrapper(_testExistingCategory);
        var handler = new DeleteCategoryHandler(_mockRepositoryWrapper.Object);

        var result = await handler.Handle(new DeleteCategoryCommand(_testExistingCategory.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Handle_ShouldNotDeleteCategory_CategoryNotFound(long categoryId)
    {
        SetupRepositoryWrapper();
        var handler = new DeleteCategoryHandler(_mockRepositoryWrapper.Object);

        var result = await handler.Handle(new DeleteCategoryCommand(categoryId), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(categoryId, typeof(Category)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldNotDeleteCategory_AnyTeamMemberDependsOnCategory()
    {
        Category categoryWithDependencies = new()
        {
            Id = 1,
            Name = "Test name",
            Description = "Test description",
            CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Local),
            TeamMembers = [new TeamMember()],
        };
        SetupRepositoryWrapper(categoryWithDependencies);
        var handler = new DeleteCategoryHandler(_mockRepositoryWrapper.Object);

        var result = await handler.Handle(new DeleteCategoryCommand(categoryWithDependencies.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(CategoryConstants.CantDeleteCategoryWhileAssociatedWithAnyTeamMember, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldNotDeleteCategory_SaveChangesFails()
    {
        SetupRepositoryWrapper(_testExistingCategory, -1);
        var handler = new DeleteCategoryHandler(_mockRepositoryWrapper.Object);

        var result = await handler.Handle(new DeleteCategoryCommand(_testExistingCategory.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntity(typeof(Category)), result.Errors[0].Message);
    }

    private void SetupRepositoryWrapper(Category? entityToDelete = null, int saveResult = 1)
    {
        _mockRepositoryWrapper.Setup(x => x.CategoriesRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<Category>>()))
            .ReturnsAsync(entityToDelete);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
