using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.BLL.Validators.Localization.TeamMembers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.TeamMembers;

public class CreateTeamMemberLocalizationTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly IValidator<CreateTeamMemberLocalizationCommand> _validator;

    private readonly CreateTeamMemberLocalizationDto _testCreateDto = new()
    {
        EntityId = 1,
        LanguageId = 1,
        FullName = "John Doe",
        Description = "Experienced developer in localization."
    };

    private readonly TeamMemberLocalization _testEntity = new()
    {
        EntityId = 1,
        Entity = new()
        {
            Id = 1,
            FullName = "TestName",
            Priority = 1,
            CategoryId = 1,
            Status = Status.Draft,
            Description = "Long description",
            Email = "Test@gmail.com",
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-10)
        },
        LanguageId = 1,
        FullName = "John Doe",
        Description = "Experienced developer in localization."
    };

    private readonly TeamMemberLocalizationDto _testDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new() { Id = 1, Code = "en" },
        FullName = "John Doe",
        Description = "Experienced developer in localization."
    };

    public CreateTeamMemberLocalizationTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _validator = new CreateTeamMemberLocalizationValidator(new BaseTeamMemberLocalizationValidator());
    }

    [Fact]
    public async Task Handle_ShouldCreateTeamMemberLocalization_Successfully()
    {
        // Arrange
        SetupDependencies(saveResult: 1);
        SetupAdditionalRepositories();
        var handler = new CreateTeamMemberLocalizationHandler(
            _mockRepositoryWrapper.Object, _mockMapper.Object, _validator);

        var command = new CreateTeamMemberLocalizationCommand(_testCreateDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_testDto.FullName, result.Value.FullName);
        Assert.Equal(_testDto.LocalizationInfoDto.Id, result.Value.LocalizationInfoDto.Id);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        SetupAdditionalRepositories();
        var invalidDto = new CreateTeamMemberLocalizationDto
        {
            EntityId = 1,
            LanguageId = 1,
            FullName = "", // invalid
            Description = "Too short"
        };

        var handler = new CreateTeamMemberLocalizationHandler(
            _mockRepositoryWrapper.Object, _mockMapper.Object, _validator);

        var command = new CreateTeamMemberLocalizationCommand(invalidDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(ErrorMessagesConstants.PropertyIsRequired(nameof(TeamMemberLocalization.FullName)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        // Arrange
        SetupDependencies(saveResult: -1);
        SetupAdditionalRepositories();
        var handler = new CreateTeamMemberLocalizationHandler(
            _mockRepositoryWrapper.Object, _mockMapper.Object, _validator);

        var command = new CreateTeamMemberLocalizationCommand(_testCreateDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(TeamMemberLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        SetupAdditionalRepositories();
        _mockMapper.Setup(x => x.Map<TeamMemberLocalization>(It.IsAny<CreateTeamMemberLocalizationDto>()))
            .Returns(_testEntity);

        _mockRepositoryWrapper.Setup(x => x.TeamMemberLocalizationsRepository.CreateAsync(It.IsAny<TeamMemberLocalization>()))
            .ThrowsAsync(new DbUpdateException());

        var handler = new CreateTeamMemberLocalizationHandler(
            _mockRepositoryWrapper.Object, _mockMapper.Object, _validator);

        var command = new CreateTeamMemberLocalizationCommand(_testCreateDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(TeamMemberLocalization)), result.Errors[0].Message);
    }

    private void SetupDependencies(int saveResult = 1)
    {
        _mockMapper.Setup(x => x.Map<TeamMemberLocalization>(It.IsAny<CreateTeamMemberLocalizationDto>()))
            .Returns(_testEntity);

        _mockMapper.Setup(x => x.Map<TeamMemberLocalizationDto>(It.IsAny<TeamMemberLocalization>()))
            .Returns(_testDto);

        _mockRepositoryWrapper.Setup(x => x.TeamMemberLocalizationsRepository.CreateAsync(It.IsAny<TeamMemberLocalization>()))
            .ReturnsAsync(_testEntity);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(saveResult);
    }

    private void SetupAdditionalRepositories()
    {
        _mockRepositoryWrapper.Setup(x => x.TeamMembersRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<TeamMember>>()))
            .ReturnsAsync(new TeamMember());

        _mockRepositoryWrapper.Setup(x => x.LocalizationLanguagesRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<LocalizationLanguage>>()))
           .ReturnsAsync(new LocalizationLanguage());
    }
}
