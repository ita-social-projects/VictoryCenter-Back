using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgram.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using HippotherapyProgramEntity = VictoryCenter.DAL.Entities.HippotherapyProgram;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyPrograms;

public class CreateHippotherapyProgramLocalizationHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IValidator<CreateHippotherapyProgramLocalizationCommand>> _mockValidator;
    private readonly Mock<ILocalizationService<HippotherapyProgramEntity, HippotherapyProgramLocalization>> _mockProgramLocalizationService;
    private readonly Mock<ILocalizationService<ProgramSectionContent, ProgramSectionContentLocalization>> _mockContentLocalizationService;
    private readonly CreateHippotherapyProgramLocalizationHandler _handler;

    private readonly CreateHippotherapyProgramLocalizationDto _testCreateDto = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Name = "Test Program",
        Description = "Test Description",
        Location = "Test Location",
        ParticipantsCount = "20",
        MeetingsCount = "10",
        Sections = new List<CreateHippotherapyProgramSectionLocalizationDto>()
    };

    private readonly HippotherapyProgramLocalization _testEntity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Name = "Test Program",
        Description = "Test Description",
        Location = "Test Location"
    };

    private readonly HippotherapyProgramLocalizationDto _testDto = new()
    {
        Name = "Test Program",
        Description = "Test Description",
        Location = "Test Location",
        LocalizationInfoDto = new() { Id = 2, Code = "en" }
    };

    private readonly HippotherapyProgramEntity _testProgram = new()
    {
        Id = 1,
        Sections = new List<HippotherapyProgramSection>()
    };

    private readonly HippotherapyProgramEntity _testProgramWithQuestion = new()
    {
        Id = 1,
        Sections = new List<HippotherapyProgramSection>
        {
            new()
            {
                Contents = new List<ProgramSectionContent>
                {
                    new TitleProgramContent { Id = 100 },
                    new DescriptionProgramContent { Id = 101 }
                }
            }
        }
    };

    public CreateHippotherapyProgramLocalizationHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockValidator = new Mock<IValidator<CreateHippotherapyProgramLocalizationCommand>>();
        _mockProgramLocalizationService = new Mock<ILocalizationService<HippotherapyProgramEntity, HippotherapyProgramLocalization>>();
        _mockContentLocalizationService = new Mock<ILocalizationService<ProgramSectionContent, ProgramSectionContentLocalization>>();

        _handler = new CreateHippotherapyProgramLocalizationHandler(
            _mockMapper.Object,
            _mockValidator.Object,
            _mockProgramLocalizationService.Object,
            _mockRepositoryWrapper.Object,
            _mockContentLocalizationService.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateHippotherapyProgramLocalization_Successfully()
    {
        // Arrange
        SetupDependencies();
        var command = new CreateHippotherapyProgramLocalizationCommand(_testCreateDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_testDto.Name, result.Value.Name);
        _mockMapper.Verify(m => m.Map<HippotherapyProgramLocalization>(It.IsAny<CreateHippotherapyProgramLocalizationDto>()), Times.Once);
        _mockProgramLocalizationService.Verify(s => s.CreateEntityLocalizationAsync(It.IsAny<HippotherapyProgramLocalization>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationErrors_WhenValidationFails()
    {
        // Arrange
        var invalidDto = new CreateHippotherapyProgramLocalizationDto
        {
            EntityId = 0,
            LanguageId = 0,
            Name = "",
            Description = "",
            Location = "",
            ParticipantsCount = "",
            MeetingsCount = "",
            Sections = new List<CreateHippotherapyProgramSectionLocalizationDto>()
        };
        var command = new CreateHippotherapyProgramLocalizationCommand(invalidDto);

        var validationFailure = new FluentValidation.Results.ValidationFailure("EntityId", "EntityId must be positive");
        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<FluentValidation.ValidationContext<CreateHippotherapyProgramLocalizationCommand>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException(new[] { validationFailure }));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenProgramNotFound()
    {
        // Arrange
        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<FluentValidation.ValidationContext<CreateHippotherapyProgramLocalizationCommand>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgramEntity>>()))
            .ReturnsAsync((HippotherapyProgramEntity)null!);

        var command = new CreateHippotherapyProgramLocalizationCommand(_testCreateDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(
            ErrorMessagesConstants.NotFound(_testCreateDto.EntityId, typeof(HippotherapyProgramEntity)),
            result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        SetupDependencies();
        _mockProgramLocalizationService
            .Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<HippotherapyProgramLocalization>()))
            .ThrowsAsync(new DbUpdateException("Database error", new InvalidOperationException()));

        var command = new CreateHippotherapyProgramLocalizationCommand(_testCreateDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(
            ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(HippotherapyProgramLocalization)),
            result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        // Arrange
        SetupDependencies();
        _mockProgramLocalizationService
            .Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<HippotherapyProgramLocalization>()))
            .ThrowsAsync(new InvalidOperationException());

        var command = new CreateHippotherapyProgramLocalizationCommand(_testCreateDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(
            ErrorMessagesConstants.FailedToCreateEntity(typeof(HippotherapyProgramLocalization)),
            result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenContentEntityIdNotFound()
    {
        // Arrange
        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<FluentValidation.ValidationContext<CreateHippotherapyProgramLocalizationCommand>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        // Program has Title content (Id 100), but we reference non-existent content (Id 999)
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgramEntity>>()))
            .ReturnsAsync(_testProgramWithQuestion);

        var dtoWithMissingContent = new CreateHippotherapyProgramLocalizationDto
        {
            EntityId = 1,
            LanguageId = 2,
            Name = "Test Program",
            Description = "Test Description",
            Location = "Test Location",
            ParticipantsCount = "20",
            MeetingsCount = "10",

            // Reference content that doesn't exist in program (program has 100, 101, but we reference 999)
            Sections = new List<CreateHippotherapyProgramSectionLocalizationDto>
            {
                new()
                {
                    Contents = new List<CreateHippotherapyProgramSectionContentLocalizationDto>
                    {
                        new() { Title = "Some Title" }
                    }
                }
            }
        };

        var command = new CreateHippotherapyProgramLocalizationCommand(dtoWithMissingContent);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
    }

    private void SetupDependencies()
    {
        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<FluentValidation.ValidationContext<CreateHippotherapyProgramLocalizationCommand>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgramEntity>>()))
            .ReturnsAsync(_testProgram);

        _mockMapper
            .Setup(m => m.Map<HippotherapyProgramLocalization>(It.IsAny<CreateHippotherapyProgramLocalizationDto>()))
            .Returns(_testEntity);

        _mockMapper
            .Setup(m => m.Map<HippotherapyProgramLocalizationDto>(It.IsAny<HippotherapyProgramLocalization>()))
            .Returns(_testDto);

        _mockProgramLocalizationService
            .Setup(s => s.CreateEntityLocalizationAsync(It.IsAny<HippotherapyProgramLocalization>()))
            .ReturnsAsync(_testEntity);
    }
}
