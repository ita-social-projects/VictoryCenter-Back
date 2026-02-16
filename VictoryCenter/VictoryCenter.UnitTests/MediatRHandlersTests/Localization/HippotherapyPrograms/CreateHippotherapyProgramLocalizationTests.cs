using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgram.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.HippotherapyPrograms;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using HippotherapyProgramEntity = VictoryCenter.DAL.Entities.HippotherapyProgram;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyPrograms;

public class CreateHippotherapyProgramLocalizationTests
{
    private readonly Mock<ILocalizationService<HippotherapyProgramEntity, HippotherapyProgramLocalization>> _mockLocalizationService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly IValidator<CreateHippotherapyProgramLocalizationCommand> _validator;

    private readonly CreateHippotherapyProgramLocalizationDto _testCreateDto = new()
    {
        EntityId = 1,
        LanguageId = 1,
        Name = "Test Program",
        Description = "This is a test hippotherapy program description.",
        Location = "Kyiv, Ukraine",
        ParticipantsCount = "10-15",
        MeetingsCount = "Twice a week"
    };

    private readonly HippotherapyProgramLocalization _testEntity = new()
    {
        EntityId = 1,
        Entity = new()
        {
            Id = 1,
            Name = "Test Program",
            Description = "Test Description",
            Status = Status.Draft,
            Slug = "test-program",
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-10)
        },
        LanguageId = 1,
        Name = "Test Program",
        Description = "This is a test hippotherapy program description.",
        Location = "Kyiv, Ukraine",
        ParticipantsCount = "10-15",
        MeetingsCount = "Twice a week"
    };

    private readonly HippotherapyProgramLocalizationDto _testDto = new()
    {
        Id = 1,
        LocalizationInfoDto = new() { Id = 1, Code = "en" },
        Name = "Test Program",
        Description = "This is a test hippotherapy program description.",
        Location = "Kyiv, Ukraine",
        ParticipantsCount = "10-15",
        MeetingsCount = "Twice a week",
        TranslationStatus = DAL.Enums.TranslationStatus.Relevant
    };

    public CreateHippotherapyProgramLocalizationTests()
    {
        _mockLocalizationService = new Mock<ILocalizationService<HippotherapyProgramEntity, HippotherapyProgramLocalization>>();
        _mockMapper = new Mock<IMapper>();
        _validator = new CreateHippotherapyProgramLocalizationValidator(new BaseHippotherapyProgramLocalizationValidator());
    }

    [Fact]
    public async Task Handle_ShouldCreateHippotherapyProgramLocalization_Successfully()
    {
        // Arrange
        SetupDependencies();
        var handler = new CreateHippotherapyProgramLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new CreateHippotherapyProgramLocalizationCommand(_testCreateDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_testDto.Name, result.Value.Name);
        Assert.Equal(_testDto.LocalizationInfoDto.Id, result.Value.LocalizationInfoDto.Id);
        _mockMapper.Verify(m => m.Map<HippotherapyProgramLocalization>(It.IsAny<CreateHippotherapyProgramLocalizationDto>()), Times.Once);
        _mockLocalizationService.Verify(s => s.CreateEntityLocalizationAsync(It.IsAny<HippotherapyProgramLocalization>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        _mockMapper.Setup(x => x.Map<HippotherapyProgramLocalization>(It.IsAny<CreateHippotherapyProgramLocalizationDto>()))
            .Returns(_testEntity);

        _mockMapper.Setup(x => x.Map<HippotherapyProgramLocalizationDto>(It.IsAny<HippotherapyProgramLocalization>()))
            .Returns(_testDto);

        _mockLocalizationService.Setup(x => x.CreateEntityLocalizationAsync(It.IsAny<HippotherapyProgramLocalization>()))
            .ThrowsAsync(new DbUpdateException());

        var handler = new CreateHippotherapyProgramLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new CreateHippotherapyProgramLocalizationCommand(_testCreateDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(HippotherapyProgramLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        // Arrange
        var notFoundMessage = "Not found";
        _mockMapper.Setup(x => x.Map<HippotherapyProgramLocalization>(It.IsAny<CreateHippotherapyProgramLocalizationDto>()))
            .Returns(_testEntity);

        _mockLocalizationService.Setup(x => x.CreateEntityLocalizationAsync(It.IsAny<HippotherapyProgramLocalization>()))
            .ThrowsAsync(new KeyNotFoundException(notFoundMessage));

        var handler = new CreateHippotherapyProgramLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new CreateHippotherapyProgramLocalizationCommand(_testCreateDto);

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
        _mockMapper.Setup(x => x.Map<HippotherapyProgramLocalization>(It.IsAny<CreateHippotherapyProgramLocalizationDto>()))
            .Returns(_testEntity);

        _mockLocalizationService.Setup(x => x.CreateEntityLocalizationAsync(It.IsAny<HippotherapyProgramLocalization>()))
            .ThrowsAsync(new InvalidOperationException());

        var handler = new CreateHippotherapyProgramLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new CreateHippotherapyProgramLocalizationCommand(_testCreateDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyProgramLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var invalidDto = new CreateHippotherapyProgramLocalizationDto
        {
            EntityId = 0, // invalid
            LanguageId = 1,
            Name = "A", // invalid - too short
            Description = "Short" // invalid - too short
        };

        var handler = new CreateHippotherapyProgramLocalizationHandler(
            _mockMapper.Object, _validator, _mockLocalizationService.Object);

        var command = new CreateHippotherapyProgramLocalizationCommand(invalidDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.Errors.Count > 0);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(x => x.Map<HippotherapyProgramLocalization>(It.IsAny<CreateHippotherapyProgramLocalizationDto>()))
            .Returns(_testEntity);

        _mockMapper.Setup(x => x.Map<HippotherapyProgramLocalizationDto>(It.IsAny<HippotherapyProgramLocalization>()))
            .Returns(_testDto);

        _mockLocalizationService.Setup(x => x.CreateEntityLocalizationAsync(It.IsAny<HippotherapyProgramLocalization>()))
            .ReturnsAsync(_testEntity);
    }
}
