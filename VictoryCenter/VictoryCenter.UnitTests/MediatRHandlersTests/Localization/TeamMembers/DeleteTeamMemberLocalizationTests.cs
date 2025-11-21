using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.TeamMembers;

public class DeleteTeamMemberLocalizationTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;

    private readonly TeamMemberLocalization _existingEntity = new()
    {
        EntityId = 1,
        LanguageId = 1,
        FullName = "Test Name",
        Description = "Test Description",
        CreatedAt = DateTime.UtcNow
    };

    public DeleteTeamMemberLocalizationTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_ShouldDeleteEntity()
    {
        SetupRepositoryWrapper(_existingEntity);
        var handler = new DeleteTeamMemberLocalizationHandler(_mockRepositoryWrapper.Object);

        var result = await handler.Handle(
            new DeleteTeamMemberLocalizationCommand(_existingEntity.EntityId, _existingEntity.LanguageId),
            CancellationToken.None);
        var response = new DeleteTeamMemberLocalizationDto
        {
            EntityId = _existingEntity.EntityId,
            LanguageId = _existingEntity.LanguageId
        };

        Assert.True(result.IsSuccess);
        Assert.Equal(response, result.Value);
    }

    [Theory]
    [InlineData(99, 99)]
    [InlineData(0, 0)]
    public async Task Handle_ShouldFail_WhenEntityNotFound(long entityId, long languageId)
    {
        SetupRepositoryWrapper(null);
        var handler = new DeleteTeamMemberLocalizationHandler(_mockRepositoryWrapper.Object);

        var result = await handler.Handle(new DeleteTeamMemberLocalizationCommand(entityId, languageId), CancellationToken.None);
        var response = new DeleteTeamMemberLocalizationDto
        {
            EntityId = entityId,
            LanguageId = languageId
        };

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(response, typeof(TeamMemberLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        SetupRepositoryWrapper(_existingEntity, -1);
        var handler = new DeleteTeamMemberLocalizationHandler(_mockRepositoryWrapper.Object);

        var result = await handler.Handle(
            new (_existingEntity.EntityId, _existingEntity.LanguageId),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntity(typeof(TeamMemberLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockRepositoryWrapper.Setup(r =>
               r.TeamMemberLocalizationsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<TeamMemberLocalization>>()))
           .ReturnsAsync(_existingEntity);
        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());
        var handler = new DeleteTeamMemberLocalizationHandler(_mockRepositoryWrapper.Object);

        var result = await handler.Handle(
            new(_existingEntity.EntityId, _existingEntity.LanguageId),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntityInDatabase(typeof(TeamMemberLocalization)), result.Errors[0].Message);
    }

    private void SetupRepositoryWrapper(TeamMemberLocalization? entityToReturn = null, int saveResult = 1)
    {
        _mockRepositoryWrapper.Setup(x => x.TeamMemberLocalizationsRepository.GetFirstOrDefaultAsync(
            It.IsAny<QueryOptions<TeamMemberLocalization>>()))
            .ReturnsAsync(entityToReturn);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(saveResult);
    }
}
