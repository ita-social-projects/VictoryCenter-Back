using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgram.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.HippotherapyPrograms;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
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
    private readonly Mock<IProgramSectionContentService> _mockProgramSectionContentService;
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

    private readonly HippotherapyProgramEntity _testProgramWithContent = new()
    {
        Id = 1,
        Sections = new List<HippotherapyProgramSection>
        {
            new()
            {
                Id = 100,
                Contents = new List<ProgramSectionContent>
                {
                    new FaqQuestionProgramContent
                    {
                        Id = 200,
                        ContentType = ContentType.FaqQuestion,
                        FaqQuestionId = 1,
                        Localizations = new List<ProgramSectionContentLocalization>
                        {
                            new()
                            {
                                EntityId = 200,
                                LanguageId = 2,
                                Language = new LocalizationLanguage { Id = 2, Code = "en" }
                            }
                        }
                    }
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
        _mockProgramSectionContentService = new Mock<IProgramSectionContentService>();

        _handler = new CreateHippotherapyProgramLocalizationHandler(
            _mockMapper.Object,
            _mockValidator.Object,
            _mockProgramLocalizationService.Object,
            _mockRepositoryWrapper.Object,
            _mockContentLocalizationService.Object,
            _mockProgramSectionContentService.Object);
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
        _mockProgramLocalizationService.Verify(s => s.TrackEntityLocalizationAsync(It.IsAny<HippotherapyProgramLocalization>()), Times.Once);
        _mockRepositoryWrapper.Verify(r => r.SaveChangesAsync(), Times.Once);
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

        _mockProgramSectionContentService
            .Setup(s => s.GetContentTypesByProgramIdAsync(It.IsAny<long>()))
            .ThrowsAsync(new KeyNotFoundException(ErrorMessagesConstants.NotFound(_testCreateDto.EntityId, typeof(HippotherapyProgramEntity))));

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
        _mockRepositoryWrapper
            .Setup(r => r.SaveChangesAsync())
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
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<HippotherapyProgramLocalization>()))
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
    public async Task Handle_ShouldFail_WhenContentTypeFieldsMismatch()
    {
        // Arrange
        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<FluentValidation.ValidationContext<CreateHippotherapyProgramLocalizationCommand>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockProgramSectionContentService
            .Setup(s => s.GetContentTypesByProgramIdAsync(It.IsAny<long>()))
            .ReturnsAsync(new Dictionary<long, ContentType>
            {
                { 200, ContentType.FaqQuestion },
                { 201, ContentType.FaqQuestion }
            });

        var dtoWithWrongFields = new CreateHippotherapyProgramLocalizationDto
        {
            EntityId = 1,
            LanguageId = 2,
            Name = "Програма",
            Description = "Опис",
            Location = "Місце",
            ParticipantsCount = "20",
            MeetingsCount = "10",

            Sections = new List<CreateHippotherapyProgramSectionLocalizationDto>
            {
                new()
                {
                    Contents = new List<CreateHippotherapyProgramSectionContentLocalizationDto>
                    {
                        new()
                        {
                            EntityId = 200,
                            Title = "Хибний тип контенту"
                        }
                    }
                }
            }
        };

        var command = new CreateHippotherapyProgramLocalizationCommand(dtoWithWrongFields);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesReturnsZero()
    {
        // Arrange
        SetupDependencies();
        _mockRepositoryWrapper
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(0);

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
    public async Task Handle_ShouldFail_WhenLanguageNotFound()
    {
        // Arrange
        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<FluentValidation.ValidationContext<CreateHippotherapyProgramLocalizationCommand>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockProgramSectionContentService
            .Setup(s => s.GetContentTypesByProgramIdAsync(It.IsAny<long>()))
            .ReturnsAsync(new Dictionary<long, ContentType>());

        _mockMapper
            .Setup(m => m.Map<HippotherapyProgramLocalization>(It.IsAny<CreateHippotherapyProgramLocalizationDto>()))
            .Returns(_testEntity);

        _mockMapper
            .Setup(m => m.Map<List<ProgramSectionContentLocalization>>(It.IsAny<List<CreateHippotherapyProgramSectionContentLocalizationDto>>()))
            .Returns(new List<ProgramSectionContentLocalization>());

        _mockProgramLocalizationService
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<HippotherapyProgramLocalization>()))
            .ThrowsAsync(new KeyNotFoundException(ErrorMessagesConstants.NotFound(_testCreateDto.LanguageId, typeof(LocalizationLanguage))));

        var command = new CreateHippotherapyProgramLocalizationCommand(_testCreateDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(
            ErrorMessagesConstants.NotFound(_testCreateDto.LanguageId, typeof(LocalizationLanguage)),
            result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUnexpectedExceptionThrown()
    {
        // Arrange
        SetupDependencies();
        _mockProgramLocalizationService
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<HippotherapyProgramLocalization>()))
            .ThrowsAsync(new Exception("Something went wrong"));

        var command = new CreateHippotherapyProgramLocalizationCommand(_testCreateDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("Unexpected error: Something went wrong", result.Errors.Select(e => e.Message));
    }

    private void SetupDependencies()
    {
        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<FluentValidation.ValidationContext<CreateHippotherapyProgramLocalizationCommand>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var programLocalizationWithSections = new HippotherapyProgramLocalization
        {
            EntityId = 1,
            LanguageId = 2,
            Entity = _testProgramWithContent
        };

        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyProgramsLocalizationsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgramLocalization>>()))
            .ReturnsAsync(programLocalizationWithSections);

        _mockRepositoryWrapper
            .Setup(r => r.LocalizationLanguagesRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<LocalizationLanguage>>()))
            .ReturnsAsync(new LocalizationLanguage
            {
                Id = 2,
                Code = "en"
            });

        _mockRepositoryWrapper
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        _mockMapper
            .Setup(m => m.Map<HippotherapyProgramLocalization>(It.IsAny<CreateHippotherapyProgramLocalizationDto>()))
            .Returns(_testEntity);

        _mockMapper
            .Setup(m => m.Map<HippotherapyProgramLocalizationDto>(It.IsAny<HippotherapyProgramLocalization>()))
            .Returns(_testDto);

        _mockMapper
            .Setup(m => m.Map<LocalizationInfoDto>(It.IsAny<LocalizationLanguage>()))
            .Returns(new LocalizationInfoDto { Id = 2, Code = "en" });

        _mockMapper
            .Setup(m => m.Map<List<ProgramSectionContentLocalization>>(It.IsAny<List<CreateHippotherapyProgramSectionContentLocalizationDto>>()))
            .Returns(new List<ProgramSectionContentLocalization>());

        _mockMapper
            .Setup(m => m.Map<HippotherapyProgramSectionContentLocalizationDto>(It.IsAny<ProgramSectionContentLocalization>()))
            .Returns(new HippotherapyProgramSectionContentLocalizationDto
            {
                EntityId = 200,
                LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" }
            });

        _mockProgramLocalizationService
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<HippotherapyProgramLocalization>()))
            .ReturnsAsync(_testEntity);

        _mockContentLocalizationService
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<List<ProgramSectionContentLocalization>>()))
            .Returns(Task.CompletedTask);

        _mockProgramSectionContentService
            .Setup(s => s.GetContentTypesByProgramIdAsync(It.IsAny<long>()))
            .ReturnsAsync(new Dictionary<long, ContentType>
            {
                { 200, ContentType.FaqQuestion },
                { 201, ContentType.FaqQuestion }
            });
    }
}
