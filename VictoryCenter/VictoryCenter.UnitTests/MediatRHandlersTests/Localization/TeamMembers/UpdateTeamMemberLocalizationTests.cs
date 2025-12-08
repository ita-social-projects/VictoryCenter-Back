using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.TeamMembers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.TeamMembers;

public class UpdateTeamMemberLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocalizationService<TeamMember, TeamMemberLocalization>> _mockLocalizationService;
    private readonly IValidator<UpdateTeamMemberLocalizationCommand> _validator;

    private readonly TeamMemberLocalization _testEntity = new()
    {
        EntityId = 1,
        LanguageId = 1,
        FullName = "Old Name",
        Description = "Old description",
        CreatedAt = DateTime.UtcNow.AddDays(-1),
        TranslationStatus = TranslationStatus.Relevant
    };

    private readonly TeamMemberLocalization _updatedEntity = new()
    {
        EntityId = 1,
        LanguageId = 1,
        FullName = "New Name",
        Description = "New description",
        CreatedAt = DateTime.UtcNow.AddDays(-1),
        TranslationStatus = TranslationStatus.Relevant
    };

    private readonly UpdateTeamMemberLocalizationDto _updatedDto = new()
    {
        FullName = "New name",
        Description = "New description",
    };

    private readonly TeamMemberLocalizationDto _updatedTestDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new() { Id = 1, Code = "en" },
        FullName = "New name",
        Description = "New description",
    };

    private readonly long _entityId = 1;
    private readonly long _languageId = 1;

    public UpdateTeamMemberLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLocalizationService = new Mock<ILocalizationService<TeamMember, TeamMemberLocalization>>();
        _validator = new UpdateTeamMemberLocalizationValidator(new BaseTeamMemberLocalizationValidator());
    }

    [Fact]
    public async Task Handle_ShouldUpdateTeamMemberLocalization_Successfully()
    {
        // Arrange
        SetupDependencies(_updatedEntity);
        var handler = new UpdateTeamMemberLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);
        long entityId = _testEntity.EntityId;
        long languageId = _testEntity.LanguageId;

        var command = new UpdateTeamMemberLocalizationCommand(_updatedDto, entityId, languageId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_updatedDto.FullName, result.Value.FullName);
        Assert.Equal(_updatedDto.Description, result.Value.Description);
        Assert.Equal(_entityId, result.Value.EntityId);
        Assert.Equal(_languageId, result.Value.LocalizationInfoDto.Id);
        _mockMapper.Verify(m => m.Map<TeamMemberLocalization>(It.IsAny<UpdateTeamMemberLocalizationDto>()), Times.Once);
        _mockLocalizationService.Verify(s => s.UpdateEntityLocalizationAsync(It.IsAny<TeamMemberLocalization>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        SetupMapper();

        _mockLocalizationService.Setup(x => x.UpdateEntityLocalizationAsync(It.IsAny<TeamMemberLocalization>()))
            .ThrowsAsync(new DbUpdateException());

        var handler = new UpdateTeamMemberLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new UpdateTeamMemberLocalizationCommand(_updatedDto, _entityId, _languageId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(TeamMemberLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        // Arrange
        var notFoundMessage = "Not found";
        SetupMapper();

        _mockLocalizationService.Setup(x => x.UpdateEntityLocalizationAsync(It.IsAny<TeamMemberLocalization>()))
            .ThrowsAsync(new KeyNotFoundException(notFoundMessage));

        var handler = new UpdateTeamMemberLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new UpdateTeamMemberLocalizationCommand(_updatedDto, _entityId, _languageId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal(notFoundMessage, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        // Arrange
        _mockMapper.Setup(x => x.Map<TeamMemberLocalization>(It.IsAny<UpdateTeamMemberLocalizationDto>()))
            .Returns(_testEntity);

        _mockLocalizationService.Setup(x => x.UpdateEntityLocalizationAsync(It.IsAny<TeamMemberLocalization>()))
            .ThrowsAsync(new InvalidOperationException());

        var handler = new UpdateTeamMemberLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new UpdateTeamMemberLocalizationCommand(_updatedDto, _entityId, _languageId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(TeamMemberLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var invalidDto = new UpdateTeamMemberLocalizationDto
        {
            FullName = "", // invalid
            Description = "Too short"
        };

        var handler = new UpdateTeamMemberLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new UpdateTeamMemberLocalizationCommand(invalidDto, _entityId, _languageId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(ErrorMessagesConstants.PropertyIsRequired(nameof(TeamMemberLocalization.FullName)), result.Errors[0].Message);
    }

    private void SetupDependencies(TeamMemberLocalization? entityToReturn = null)
    {
        SetupMapper();
        SetupLocalizationService(entityToReturn);
    }

    private void SetupLocalizationService(TeamMemberLocalization? entityToReturn = null)
    {
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(It.IsAny<TeamMemberLocalization>()))
            .ReturnsAsync(entityToReturn);
    }

    private void SetupMapper()
    {
        _mockMapper.Setup(m => m.Map<TeamMemberLocalization>(It.IsAny<UpdateTeamMemberLocalizationDto>()))
            .Returns(_updatedEntity);

        _mockMapper.Setup(m => m.Map(It.IsAny<UpdateTeamMemberLocalizationDto>(), It.IsAny<TeamMemberLocalization>()))
            .Returns(_updatedEntity);

        _mockMapper.Setup(m => m.Map<TeamMemberLocalizationDto>(It.IsAny<TeamMemberLocalization>()))
            .Returns(_updatedTestDto);
    }
}
