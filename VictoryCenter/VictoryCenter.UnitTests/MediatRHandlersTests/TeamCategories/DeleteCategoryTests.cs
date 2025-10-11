using Moq;
using VictoryCenter.BLL.Commands.Admin.TeamCategories.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.TeamCategories;

public class DeleteCategoryTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly TeamCategory _testExistingCategory = new()
    {
        Id = 1,
        Name = "Test name",
        Description = "Test description",
        CreatedAt = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeZoneInfo.Local.BaseUtcOffset),
    };

    public DeleteCategoryTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldDeleteCategory()
    {
        SetupRepositoryWrapper(_testExistingCategory);
        var handler = new DeleteTeamCategoryHandler(_mockRepositoryWrapper.Object);

        var result = await handler.Handle(new DeleteTeamCategoryCommand(_testExistingCategory.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Handle_ShouldNotDeleteCategory_CategoryNotFound(long categoryId)
    {
        SetupRepositoryWrapper();
        var handler = new DeleteTeamCategoryHandler(_mockRepositoryWrapper.Object);

        var result = await handler.Handle(new DeleteTeamCategoryCommand(categoryId), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(categoryId, typeof(TeamCategory)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldNotDeleteCategory_AnyTeamMemberDependsOnCategory()
    {
        TeamCategory categoryWithDependencies = new()
        {
            Id = 1,
            Name = "Test name",
            Description = "Test description",
            CreatedAt = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeZoneInfo.Utc.BaseUtcOffset),
            TeamMembers = [new TeamMember()],
        };
        SetupRepositoryWrapper(categoryWithDependencies);
        var handler = new DeleteTeamCategoryHandler(_mockRepositoryWrapper.Object);

        var result = await handler.Handle(new DeleteTeamCategoryCommand(categoryWithDependencies.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(TeamCategoryConstants.CantDeleteCategoryWhileAssociatedWithAnyTeamMember, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldNotDeleteCategory_SaveChangesFails()
    {
        SetupRepositoryWrapper(_testExistingCategory, -1);
        var handler = new DeleteTeamCategoryHandler(_mockRepositoryWrapper.Object);

        var result = await handler.Handle(new DeleteTeamCategoryCommand(_testExistingCategory.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntity(typeof(TeamCategory)), result.Errors[0].Message);
    }

    private void SetupRepositoryWrapper(TeamCategory? entityToDelete = null, int saveResult = 1)
    {
        _mockRepositoryWrapper.Setup(x => x.TeamCategoriesRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<TeamCategory>>()))
            .ReturnsAsync(entityToDelete);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}
