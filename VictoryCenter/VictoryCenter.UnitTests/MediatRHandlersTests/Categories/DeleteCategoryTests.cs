using System.Linq.Expressions;
using Moq;
using VictoryCenter.BLL.Commands.Categories.Delete;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

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
    public async Task Handle_ShouldNotDeleteCategory_CategoryNotFound(int categoryId)
    {
        SetupRepositoryWrapper();
        var handler = new DeleteCategoryHandler(_mockRepositoryWrapper.Object);
        
        var result = await handler.Handle(new DeleteCategoryCommand(categoryId), CancellationToken.None);
        
        Assert.False(result.IsSuccess);
        Assert.Equal("Not found", result.Errors[0].Message);
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
        Assert.Equal("Can't delete category while assotiated with any team member", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldNotDeleteCategory_SaveChangesFails()
    {
        SetupRepositoryWrapper(_testExistingCategory, -1);
        var handler = new DeleteCategoryHandler(_mockRepositoryWrapper.Object);
        
        var result = await handler.Handle(new DeleteCategoryCommand(_testExistingCategory.Id), CancellationToken.None);
        
        Assert.False(result.IsSuccess);
        Assert.Equal("Failed to delete category", result.Errors[0].Message);
    }

    private void SetupRepositoryWrapper(Category? entityToDelete = null, int saveResult = 1)
    {
        _mockRepositoryWrapper.Setup(x => x.CategoriesRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Category, bool>>>()))
            .ReturnsAsync(entityToDelete);
        
        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
