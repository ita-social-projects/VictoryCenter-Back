using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.TeamMembers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.TeamMembers;

public class CreateTeamMemberLocalizationTests
{
    private readonly Mock<ILocalizationService<TeamMember, TeamMemberLocalization>> _mockLocalizationService;
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
        _mockLocalizationService = new Mock<ILocalizationService<TeamMember, TeamMemberLocalization>>();
        _mockMapper = new Mock<IMapper>();
        _validator = new CreateTeamMemberLocalizationValidator(new BaseTeamMemberLocalizationValidator());
    }

    [Fact]
    public async Task Handle_ShouldCreateTeamMemberLocalization_Successfully()
    {
        // Arrange
        SetupDependencies();
        var handler = new CreateTeamMemberLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new CreateTeamMemberLocalizationCommand(_testCreateDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_testDto.FullName, result.Value.FullName);
        Assert.Equal(_testDto.LocalizationInfoDto.Id, result.Value.LocalizationInfoDto.Id);
        _mockMapper.Verify(m => m.Map<TeamMemberLocalization>(It.IsAny<CreateTeamMemberLocalizationDto>()), Times.Once);
        _mockLocalizationService.Verify(s => s.CreateEntityLocalizationAsync(It.IsAny<TeamMemberLocalization>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        _mockMapper.Setup(x => x.Map<TeamMemberLocalization>(It.IsAny<CreateTeamMemberLocalizationDto>()))
            .Returns(_testEntity);

        _mockMapper.Setup(x => x.Map<TeamMemberLocalizationDto>(It.IsAny<TeamMemberLocalization>()))
            .Returns(_testDto);

        _mockLocalizationService.Setup(x => x.CreateEntityLocalizationAsync(It.IsAny<TeamMemberLocalization>()))
            .ThrowsAsync(new DbUpdateException());

        var handler = new CreateTeamMemberLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new CreateTeamMemberLocalizationCommand(_testCreateDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(TeamMemberLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        // Arrange
        var notFoundMessage = "Not found";
        _mockMapper.Setup(x => x.Map<TeamMemberLocalization>(It.IsAny<CreateTeamMemberLocalizationDto>()))
            .Returns(_testEntity);

        _mockLocalizationService.Setup(x => x.CreateEntityLocalizationAsync(It.IsAny<TeamMemberLocalization>()))
            .ThrowsAsync(new KeyNotFoundException(notFoundMessage));

        var handler = new CreateTeamMemberLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new CreateTeamMemberLocalizationCommand(_testCreateDto);

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
        _mockMapper.Setup(x => x.Map<TeamMemberLocalization>(It.IsAny<CreateTeamMemberLocalizationDto>()))
            .Returns(_testEntity);

        _mockLocalizationService.Setup(x => x.CreateEntityLocalizationAsync(It.IsAny<TeamMemberLocalization>()))
            .ThrowsAsync(new InvalidOperationException());

        var handler = new CreateTeamMemberLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new CreateTeamMemberLocalizationCommand(_testCreateDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToCreateEntity(typeof(TeamMemberLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var invalidDto = new CreateTeamMemberLocalizationDto
        {
            EntityId = 1,
            LanguageId = 1,
            FullName = "", // invalid
            Description = "Too short"
        };

        var handler = new CreateTeamMemberLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new CreateTeamMemberLocalizationCommand(invalidDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(ErrorMessagesConstants.PropertyIsRequired(nameof(TeamMemberLocalization.FullName)), result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(x => x.Map<TeamMemberLocalization>(It.IsAny<CreateTeamMemberLocalizationDto>()))
            .Returns(_testEntity);

        _mockMapper.Setup(x => x.Map<TeamMemberLocalizationDto>(It.IsAny<TeamMemberLocalization>()))
            .Returns(_testDto);

        _mockLocalizationService.Setup(x => x.CreateEntityLocalizationAsync(It.IsAny<TeamMemberLocalization>()))
            .ReturnsAsync(_testEntity);
    }
}
