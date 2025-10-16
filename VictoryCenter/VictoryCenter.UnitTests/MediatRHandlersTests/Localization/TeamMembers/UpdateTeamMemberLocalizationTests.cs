using AutoMapper;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.BLL.Validators.Localization.TeamMembers;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.TeamMembers;

public class UpdateTeamMemberLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly IValidator<UpdateTeamMemberLocalizationCommand> _validator;

    private readonly TeamMemberLocalization _existingEntity = new()
    {
        EntityId = 1,
        LanguageId = 1,
        FullName = "Old Name",
        Description = "Old description",
        CreatedAt = DateTime.UtcNow.AddDays(-1)
    };

    private readonly TeamMemberLocalization _updatedEntity = new()
    {
        EntityId = 1,
        LanguageId = 1,
        FullName = "New Name",
        Description = "New description",
        CreatedAt = DateTime.UtcNow.AddDays(-1)
    };

    private TeamMemberLocalizationDto _updatedDto = new()
    {
        TeamMemberId = 1,
        LocalizationLanguageDto = new() { Id = 1, Code = "en" },
        FullName = "New name",
        Description = "New description"
    };

    public UpdateTeamMemberLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _validator = new UpdateTeamMemberLocalizationValidator(new BaseTeamMemberLocalizationValidator());
    }

    [Fact]
    public async Task Handle_ShouldUpdateEntity()
    {
        SetupDependencies(_existingEntity);
        var handler = new UpdateTeamMemberLocalizationHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        var command = new UpdateTeamMemberLocalizationCommand(new UpdateTeamMemberLocalizationDto
        {
            TeamMemberId = 1,
            LanguageId = 1,
            FullName = "New Name",
            Description = "New description"
        });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_updatedDto.FullName, result.Value.FullName);
        Assert.Equal(_updatedDto.Description, result.Value.Description);
    }

    [Fact]
    public async Task Handle_ShouldNotUpdateEntity_NotFound()
    {
        SetupDependencies(null);
        var handler = new UpdateTeamMemberLocalizationHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        var dtoIds = new { TeamMemberId = 99, LanguageId = 99 };

        var command = new UpdateTeamMemberLocalizationCommand(new UpdateTeamMemberLocalizationDto
        {
            TeamMemberId = dtoIds.TeamMemberId,
            LanguageId = dtoIds.LanguageId,
            FullName = "New Name",
            Description = "New description"
        });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(ErrorMessagesConstants.NotFound(dtoIds, typeof(TeamMemberLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldNotUpdateEntity_SaveChangesFails()
    {
        SetupDependencies(_existingEntity, -1);
        var handler = new UpdateTeamMemberLocalizationHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validator);

        var command = new UpdateTeamMemberLocalizationCommand(new UpdateTeamMemberLocalizationDto
        {
            TeamMemberId = 1,
            LanguageId = 1,
            FullName = "New Name",
            Description = "New description"
        });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(ErrorMessagesConstants.FailedToUpdateEntity(typeof(TeamMemberLocalization)), result.Errors[0].Message);
    }

    private void SetupDependencies(TeamMemberLocalization? entityToReturn, int saveResult = 1)
    {
        _mockRepositoryWrapper.Setup(x => x.TeamMemberLocalizationsRepository.GetFirstOrDefaultAsync(
            It.IsAny<QueryOptions<TeamMemberLocalization>>()))
            .ReturnsAsync(entityToReturn);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(saveResult);

        _mockMapper.Setup(m => m.Map(It.IsAny<UpdateTeamMemberLocalizationDto>(), It.IsAny<TeamMemberLocalization>()))
            .Returns(_updatedEntity);

        _mockMapper.Setup(m => m.Map<TeamMemberLocalizationDto>(It.IsAny<TeamMemberLocalization>()))
            .Returns(_updatedDto);
    }
}
