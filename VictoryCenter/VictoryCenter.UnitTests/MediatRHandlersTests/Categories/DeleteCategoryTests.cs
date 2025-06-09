using System.Linq.Expressions;
using Moq;
using VictoryCenter.BLL.Commands.Categories.DeleteCategory;
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
        CreatedAt = DateTime.Now,
    };

    public DeleteCategoryTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldDeleteCategory()
    {
        SetupRepositoryWrapper(1, _testExistingCategory);
        var handler = new DeleteCategoryHandler(_mockRepositoryWrapper.Object);

        var result = await handler.Handle(new DeleteCategoryCommand(1), CancellationToken.None);
        
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
    }

    [Fact]
    public async Task Handle_ShouldNotDeleteCategory_SaveChangesFails()
    {
        SetupRepositoryWrapper(-1, _testExistingCategory);
        var handler = new DeleteCategoryHandler(_mockRepositoryWrapper.Object);
        
        var result = await handler.Handle(new DeleteCategoryCommand(1), CancellationToken.None);
        
        Assert.False(result.IsSuccess);
    }

    private void SetupRepositoryWrapper(int saveResult = 1, Category? entityToDelete = null)
    {
        _mockRepositoryWrapper.Setup(x => x.CategoriesRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Category, bool>>>()))
            .ReturnsAsync(entityToDelete);
        
        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
