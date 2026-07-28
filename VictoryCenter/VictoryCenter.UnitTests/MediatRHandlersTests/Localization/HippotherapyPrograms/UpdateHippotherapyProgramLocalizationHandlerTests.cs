using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgram.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Update;
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

public class UpdateHippotherapyProgramLocalizationHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IValidator<UpdateHippotherapyProgramLocalizationCommand>> _mockValidator;
    private readonly Mock<ILocalizationService<HippotherapyProgramEntity, HippotherapyProgramLocalization>> _mockProgramLocalizationService;
    private readonly Mock<ILocalizationService<ProgramSectionContent, ProgramSectionContentLocalization>> _mockContentLocalizationService;
    private readonly Mock<IProgramSectionContentService> _mockProgramSectionContentService;
    private readonly Mock<TimeProvider> _mockTimeProvider;
    private readonly UpdateHippotherapyProgramLocalizationHandler _handler;

    private readonly UpdateHippotherapyProgramLocalizationDto _testUpdateDto = new()
    {
        Name = "Updated Program",
        Description = "Updated Description",
        Location = "Updated Location",
        ParticipantsCount = "25",
        MeetingsCount = "12",
        Sections = new List<UpdateHippotherapyProgramSectionLocalizationDto>
        {
            new()
            {
                EntityId = 100,
                Contents = new List<UpdateHippotherapyProgramSectionContentLocalizationDto>
                {
                    new()
                    {
                        EntityId = 200,
                        Question = "Updated localized question",
                        Answer = "Updated localized answer"
                    }
                }
            }
        }
    };

    private readonly HippotherapyProgramLocalization _testEntity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Name = "Updated Program",
        Description = "Updated Description",
        Location = "Updated Location"
    };

    private readonly HippotherapyProgramLocalizationDto _testDto = new()
    {
        Name = "Updated Program",
        Description = "Updated Description",
        Location = "Updated Location",
        LocalizationInfoDto = new() { Id = 2, Code = "en" }
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
                    new TestProgramSectionContent
                    {
                        Id = 200,
                        ContentType = ContentType.FaqQuestion
                    }
                }
            }
        }
    };

    public UpdateHippotherapyProgramLocalizationHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockValidator = new Mock<IValidator<UpdateHippotherapyProgramLocalizationCommand>>();
        _mockProgramLocalizationService = new Mock<ILocalizationService<HippotherapyProgramEntity, HippotherapyProgramLocalization>>();
        _mockContentLocalizationService = new Mock<ILocalizationService<ProgramSectionContent, ProgramSectionContentLocalization>>();
        _mockProgramSectionContentService = new Mock<IProgramSectionContentService>();
        _mockTimeProvider = new Mock<TimeProvider>();
        _mockTimeProvider.Setup(x => x.GetUtcNow()).Returns(DateTimeOffset.UtcNow);

        _handler = new UpdateHippotherapyProgramLocalizationHandler(
            _mockMapper.Object,
            _mockRepositoryWrapper.Object,
            _mockValidator.Object,
            _mockProgramSectionContentService.Object,
            _mockProgramLocalizationService.Object,
            _mockContentLocalizationService.Object,
            _mockTimeProvider.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateHippotherapyProgramLocalization_Successfully()
    {
        // Arrange
        SetupDependencies();
        var command = new UpdateHippotherapyProgramLocalizationCommand(_testUpdateDto, 1, 2);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_testDto.Name, result.Value.Name);
        _mockMapper.Verify(m => m.Map<HippotherapyProgramLocalization>(It.IsAny<UpdateHippotherapyProgramLocalizationDto>()), Times.Once);
        _mockProgramLocalizationService.Verify(s => s.TrackEntityLocalizationForUpdateAsync(It.IsAny<HippotherapyProgramLocalization>()), Times.Once);
        _mockRepositoryWrapper.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationErrors_WhenValidationFails()
    {
        // Arrange
        var invalidDto = new UpdateHippotherapyProgramLocalizationDto
        {
            Name = string.Empty,
            Description = string.Empty,
            Location = string.Empty,
            ParticipantsCount = string.Empty,
            MeetingsCount = string.Empty,
            Sections = []
        };
        var command = new UpdateHippotherapyProgramLocalizationCommand(invalidDto, 0, 0);

        var validationFailure = new FluentValidation.Results.ValidationFailure("EntityId", "EntityId must be positive");
        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<FluentValidation.ValidationContext<UpdateHippotherapyProgramLocalizationCommand>>(),
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
                It.IsAny<FluentValidation.ValidationContext<UpdateHippotherapyProgramLocalizationCommand>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockProgramSectionContentService
            .Setup(s => s.GetContentTypesByProgramIdAsync(It.IsAny<long>()))
            .ReturnsAsync(new Dictionary<long, ContentType>());

        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgramEntity>>()))
            .ReturnsAsync((HippotherapyProgramEntity?)null);

        var command = new UpdateHippotherapyProgramLocalizationCommand(_testUpdateDto, 1, 2);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Not found programEntity", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        SetupDependencies();
        _mockRepositoryWrapper
            .Setup(r => r.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException("Database error", new InvalidOperationException()));

        var command = new UpdateHippotherapyProgramLocalizationCommand(_testUpdateDto, 1, 2);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HippotherapyProgramLocalization)),
            result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        // Arrange
        SetupDependencies();
        _mockProgramLocalizationService
            .Setup(s => s.TrackEntityLocalizationForUpdateAsync(It.IsAny<HippotherapyProgramLocalization>()))
            .ThrowsAsync(new InvalidOperationException());

        var command = new UpdateHippotherapyProgramLocalizationCommand(_testUpdateDto, 1, 2);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyProgramLocalization)),
            result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesReturnsZero()
    {
        // Arrange
        SetupDependencies();
        _mockRepositoryWrapper
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(0);

        var command = new UpdateHippotherapyProgramLocalizationCommand(_testUpdateDto, 1, 2);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HippotherapyProgramLocalization)),
            result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenProgramSectionContentServiceThrowsKeyNotFound()
    {
        // Arrange
        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<FluentValidation.ValidationContext<UpdateHippotherapyProgramLocalizationCommand>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgramEntity>>()))
            .ReturnsAsync(_testProgramWithContent);
        var expectedError = ErrorMessagesConstants.NotFound(1, typeof(HippotherapyProgramEntity));
        _mockProgramSectionContentService
            .Setup(s => s.GetContentTypesByProgramIdAsync(It.IsAny<long>()))
            .ThrowsAsync(new KeyNotFoundException(expectedError));

        var command = new UpdateHippotherapyProgramLocalizationCommand(_testUpdateDto, 1, 2);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(expectedError, result.Errors.Select(e => e.Message));
    }

    private void SetupDependencies()
    {
        _mockValidator
            .Setup(v => v.ValidateAsync(
                It.IsAny<FluentValidation.ValidationContext<UpdateHippotherapyProgramLocalizationCommand>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockProgramSectionContentService
            .Setup(s => s.GetContentTypesByProgramIdAsync(It.IsAny<long>()))
            .ReturnsAsync(new Dictionary<long, ContentType>
            {
                { 200, ContentType.FaqQuestion }
            });

        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyProgramsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<HippotherapyProgramEntity>>()))
            .ReturnsAsync(_testProgramWithContent);

        _mockRepositoryWrapper
            .Setup(r => r.LocalizationLanguagesRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<LocalizationLanguage>>()))
            .ReturnsAsync(new LocalizationLanguage
            {
                Id = 2,
                Code = "en"
            });

        _mockRepositoryWrapper
            .Setup(r => r.ProgramSectionContentLocalizationsRepository.GetAllAsync(It.IsAny<QueryOptions<ProgramSectionContentLocalization>>()))
            .ReturnsAsync([]);

        _mockRepositoryWrapper
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        _mockMapper
            .Setup(m => m.Map<HippotherapyProgramLocalization>(It.IsAny<UpdateHippotherapyProgramLocalizationDto>()))
            .Returns(_testEntity);

        _mockMapper
            .Setup(m => m.Map<HippotherapyProgramLocalizationDto>(It.IsAny<HippotherapyProgramLocalization>()))
            .Returns(_testDto);

        _mockMapper
            .Setup(m => m.Map<LocalizationInfoDto>(It.IsAny<LocalizationLanguage>()))
            .Returns(new LocalizationInfoDto { Id = 2, Code = "en" });

        _mockMapper
            .Setup(m => m.Map<List<ProgramSectionContentLocalization>>(It.IsAny<List<UpdateHippotherapyProgramSectionContentLocalizationDto>>()))
            .Returns(new List<ProgramSectionContentLocalization>
            {
                new()
            });

        _mockProgramLocalizationService
            .Setup(s => s.TrackEntityLocalizationForUpdateAsync(It.IsAny<HippotherapyProgramLocalization>()))
            .Returns(Task.CompletedTask);

        _mockContentLocalizationService
            .Setup(s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<ProgramSectionContentLocalization>>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        _mockProgramSectionContentService
            .Setup(s => s.GetProgramSectionsAsync(It.IsAny<long>(), It.IsAny<long>()))
            .ReturnsAsync(new List<HippotherapyProgramSectionLocalizationDto>());
    }

    private sealed class TestProgramSectionContent : ProgramSectionContent
    {
    }
}
