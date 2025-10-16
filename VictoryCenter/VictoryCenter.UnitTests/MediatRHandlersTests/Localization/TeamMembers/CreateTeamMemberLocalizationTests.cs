using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.BLL.Validators.Localization.TeamMembers;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.TeamMembers;

public class CreateTeamMemberLocalizationTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly IValidator<CreateTeamMemberLocalizationCommand> _validator;

    private readonly CreateTeamMemberLocalizationDto _testCreateDto = new()
    {
        TeamMemberId = 1,
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
        TeamMemberId = 1,
        LocalizationLanguageDto = new() { Id = 1, Code = "en" },
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
        var handler = new CreateTeamMemberLocalizationHandler(
            _mockRepositoryWrapper.Object, _mockMapper.Object, _validator);

        var command = new CreateTeamMemberLocalizationCommand(_testCreateDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_testDto.FullName, result.Value.FullName);
        Assert.Equal(_testDto.LocalizationLanguageDto.Id, result.Value.LocalizationLanguageDto.Id);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var invalidDto = new CreateTeamMemberLocalizationDto
        {
            TeamMemberId = 1,
            LanguageId = 0, // invalid
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
        Assert.Contains(ErrorMessagesConstants.PropertyMustBePositive(nameof(TeamMemberLocalization.Entity.CategoryId)), result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        // Arrange
        SetupDependencies(saveResult: -1);
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
}
